namespace Sapher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Dtos;
    using Exceptions;
    using Handlers;
    using Logger;
    using Logger.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;
    using Utils;

    internal class SapherStep : ISapherStep
    {
        public string StepName { get; set; }

        private readonly ISapherStepConfiguration configuration;
        private Type inputMessageType;
        private Type inputHandlerType;
        private ISapherDataRepository dataRepository;
        private IServiceProvider serviceProvider;
        private ILogger logger;
        private IDictionary<Type, Type> responseHandlers = new Dictionary<Type, Type>();

        internal SapherStep(ISapherStepConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Init(IServiceProvider serviceProvider, ILogger logger)
        {
            this.StepName = this.configuration.StepName;
            this.inputMessageType = this.configuration.InputMessageType;
            this.inputHandlerType = this.configuration.InputHandlerType;
            this.serviceProvider = serviceProvider;
            this.logger = logger;

            this.SetupDataRepository();
            this.SetupResponseHandlers();
        }

        public async Task<StepResult> Deliver<T>(T message, MessageSlip messageSlip) where T : class
        {
            var messageType = typeof(T);

            var stepHandlesMessageAsInput = messageType == this.inputMessageType;
            var stepHandleMessageAsResponse = this.responseHandlers
                .TryGetValue(messageType, out var responseHandlerType);

            StepResult stepResult = null;

            if (stepHandlesMessageAsInput || stepHandleMessageAsResponse)
            {
                stepResult = new StepResult(this.StepName);
            }

            if (stepHandlesMessageAsInput)
            {
                var internalCorrelationId = Guid.NewGuid().ToString();

                this.logger.Verbose(
                    $"Found Sapher Input Handler for {messageType.Name}",
                    Pair.Of("StepName", this.StepName),
                    Pair.Of("InputHandler", this.inputHandlerType.Name),
                    Pair.Of("InternalCorrelationId", internalCorrelationId));

                stepResult.InputHandlerResult = await this
                    .HandleInput(message, messageSlip, internalCorrelationId)
                    .ConfigureAwait(false);

                this.logger.Verbose(
                    $"Executed Sapher Input Handler for {messageType.Name}",
                    Pair.Of("StepName", this.StepName),
                    Pair.Of("InputHandler", this.inputHandlerType.Name),
                    Pair.Of("InternalCorrelationId", internalCorrelationId));
            }

            if (stepHandleMessageAsResponse)
            {
                var sapherCorrelationId = Guid.NewGuid().ToString();

                this.logger.Verbose(
                    $"Found Sapher Response Handler for {messageType.Name}",
                    Pair.Of("StepName", this.StepName),
                    Pair.Of("ResponseHandler", responseHandlerType.Name),
                    Pair.Of("SapherCorrelationId", sapherCorrelationId));

                stepResult.ResponseHandlerResult = await this
                    .HandleResponse(responseHandlerType, message, messageSlip, sapherCorrelationId)
                    .ConfigureAwait(false);

                this.logger.Verbose(
                    $"Executed Sapher Response Handler for {messageType.Name}",
                    Pair.Of("StepName", this.StepName),
                    Pair.Of("ResponseHandler", responseHandlerType.Name),
                    Pair.Of("SapherCorrelationId", sapherCorrelationId));
            }

            return stepResult;
        }

        private async Task<InputResult> HandleInput<T>(
            T inputMessage,
            MessageSlip messageSlip,
            string sapherCorrelationId)
            where T : class
        {
            // Idempotency
            var data = await this.dataRepository
                .GetStepInstanceFromInputMessageId(this.StepName, messageSlip.MessageId)
                .ConfigureAwait(false);

            if (data != null)
            {
                throw new SapherException(
                    "Input Message received twice. Ignoring",
                    Pair.Of("MessageType", typeof(T).Name),
                    Pair.Of("StepName", this.StepName),
                    Pair.Of("MessageId", messageSlip.MessageId),
                    Pair.Of("Step State", data.State.ToString()),
                    Pair.Of("SapherCorrelationId", sapherCorrelationId));
            }

            data = new Dtos.SapherStepData(messageSlip, this.StepName);

            var inputHandler = this.serviceProvider
                .GetServices<IHandlesInput<T>>()
                .FirstOrDefault(h => h.GetType() == this.inputHandlerType);

            if (inputHandler == null)
            {
                throw new SapherConfigurationException(
                    "An error has occurred with this Step Configuration. IHandlesInput implementation not found for specified message",
                    Pair.Of("ExpectedInputHandlerType", this.inputHandlerType.Name),
                    Pair.Of("MessageType", typeof(T).Name),
                    Pair.Of("SapherCorrelationId", sapherCorrelationId));
            }

            var result = await inputHandler
                .Execute(inputMessage, messageSlip)
                .ConfigureAwait(false);

            result = result ?? new InputResult { State = InputResultState.Failed };
            result.ExecutedHandlerName = this.inputHandlerType.Name;

            data.DataToPersist = result.DataToPersist ?? data.DataToPersist;
            data.MessagesWaitingResponse = result.SentMessageIds.ToList();

            if (result.State == Dtos.InputResultState.Successful)
            {
                data.State = result.SentMessageIds.Any()
                    ? Dtos.StepState.ExecutedInput
                    : Dtos.StepState.Successful;
            }
            else
            {
                data.State = Dtos.StepState.FailedOnExecution;
            }

            await this.dataRepository
                .Save(data)
                .ConfigureAwait(false);

            return result;
        }

        private async Task<ResponseResult> HandleResponse<T>(
            Type responseHandlerType,
            T successMessage,
            MessageSlip messageSlip,
            string sapherCorrelationId)
            where T : class
        {
            var responseHandler = this.serviceProvider
                .GetServices<IHandlesResponse<T>>()
                .FirstOrDefault(h => h.GetType() == responseHandlerType);

            if (responseHandler == null)
            {
                throw new SapherConfigurationException(
                    "An error has occurred with this Step Configuration. IHandlesResponse implementation not found for specified message",
                    Pair.Of("ExpectedResponseHandlerType", responseHandlerType.Name),
                    Pair.Of("MessageType", typeof(T).Name),
                    Pair.Of("SapherCorrelationId", sapherCorrelationId));
            }

            var data = await this.dataRepository
                .GetStepInstanceFromOutputMessageId(this.StepName, messageSlip.ConversationId)
                .ConfigureAwait(false);

            if (data != null)
            {
                if (data.IsFinished)
                {
                    throw new SapherException(
                        "This step is no longer waiting for responses. Ignoring",
                        Pair.Of("MessageType", typeof(T).Name),
                        Pair.Of("StepName", this.StepName),
                        Pair.Of("MessageId", messageSlip.MessageId),
                        Pair.Of("Step State", data.State.ToString()),
                        Pair.Of("SapherCorrelationId", sapherCorrelationId));
                }

                if (!data.IsExpectingResponses)
                {
                    throw new SapherException(
                        "This step execution does not expect any response. Ignoring",
                        Pair.Of("MessageType", typeof(T).Name),
                        Pair.Of("StepName", this.StepName),
                        Pair.Of("MessageId", messageSlip.MessageId),
                        Pair.Of("Step State", data.State.ToString()),
                        Pair.Of("SapherCorrelationId", sapherCorrelationId));
                }

                if (data.IsMessageAlreadyProcessed(messageSlip.ConversationId))
                {
                    throw new SapherException(
                        "This response message was already processed. Ignoring",
                        Pair.Of("MessageType", typeof(T).Name),
                        Pair.Of("StepName", this.StepName),
                        Pair.Of("MessageId", messageSlip.MessageId),
                        Pair.Of("Step State", data.State.ToString()),
                        Pair.Of("SapherCorrelationId", sapherCorrelationId));
                }

                var result = await responseHandler
                    .Execute(successMessage, messageSlip, data.DataToPersist)
                    .ConfigureAwait(false);

                result.ExecutedHandlerName = responseHandler.GetType().Name;
                UpdateDataAfterResponseExecution(data, result, messageSlip);
                return result;
            }

            return null;
        }

        private void UpdateDataAfterResponseExecution(
            Dtos.SapherStepData data,
            ResponseResult result,
            MessageSlip messageSlip)
        {
            data.DataToPersist = result.DataToPersist ?? data.DataToPersist;
            data.MessagesWaitingResponse.Remove(messageSlip.ConversationId);

            switch (result.State)
            {
                case ResponseResultState.Successful:
                    data.SuccessfulMessages.Add(messageSlip.ConversationId);
                    break;

                case ResponseResultState.Compensated:
                    data.CompensatedMessages.Add(messageSlip.ConversationId);
                    break;

                case ResponseResultState.Failed:
                    data.FailedMessages.Add(messageSlip.ConversationId);
                    break;
            }

            data.UpdateDate = DateTime.UtcNow;

            EvaluateStepState(data);
            this.dataRepository.Save(data);
        }

        private void EvaluateStepState(Dtos.SapherStepData data)
        {
            if (data.IsExpectingResponses)
            {
                return;
            }

            if (data.FailedMessages.Count > 0)
            {
                data.State = Dtos.StepState.FailedOnResponses;
            }
            else if (data.CompensatedMessages.Count > 0)
            {
                data.State = Dtos.StepState.Compensated;
            }
            else
            {
                data.State = Dtos.StepState.Successful;
            }
        }

        internal void SetupResponseHandlers()
        {
            if (this.configuration.ResponseHandlers?.Count > 0)
            {
                this.responseHandlers = this.configuration.ResponseHandlers;
            }
        }

        internal void SetupDataRepository()
        {
            this.dataRepository = serviceProvider.GetRequiredService<ISapherDataRepository>();
        }
    }
}