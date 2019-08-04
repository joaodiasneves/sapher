namespace Sapher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Dtos;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Persistence;
    using Model = Persistence.Model;

    public class SapherStep : ISapherStep
    {
        // TODO - Provide a Job executing in the background to check for Steps that never received all the answers
        public string StepName { get; set; }

        private readonly ISapherStepConfiguration configuration;
        private Type inputMessageType;
        private Type inputHandlerType;
        private ISapherDataRepository dataRepository;
        private IServiceProvider serviceProvider;
        private IDictionary<Type, Type> responseHandlers = new Dictionary<Type, Type>();

        internal SapherStep(ISapherStepConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Init(IServiceProvider serviceProvider)
        {
            this.StepName = this.configuration.StepName;
            this.inputMessageType = this.configuration.InputMessageType;
            this.inputHandlerType = this.configuration.InputHandlerType;
            this.serviceProvider = serviceProvider;

            this.SetupDataRepository();
            this.SetupResponseHandlers();
        }

        public async Task<StepResult> Deliver<T>(T message, MessageSlip messageSlip) where T : class
        {
            // TODO  add logs.
            // TODO  trycatches
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
                stepResult.InputHandlerResult = await this
                    .HandleInput(message, messageSlip)
                    .ConfigureAwait(false);
            }

            if (stepHandleMessageAsResponse)
            {
                stepResult.ResponseHandlerResult = await this
                    .HandleResponse(responseHandlerType, message, messageSlip)
                    .ConfigureAwait(false);
            }

            return stepResult;
        }

        private async Task<InputResult> HandleInput<T>(
            T inputMessage,
            MessageSlip messageSlip)
            where T : class
        {
            // TODO Simplify method?
            // Idempotency
            // TODO ensure versioning
            var data = await this.dataRepository
                .Load(this.StepName, messageSlip.MessageId)
                .ConfigureAwait(false);

            if (data != null)
            {
                // TODO LOG
                // TODO Throw exception (?)
                return null;
            }

            data = new Dtos.SapherStepData(messageSlip, this.StepName);

            var inputHandler = this.serviceProvider
                .GetServices<IHandlesInput<T>>()
                .FirstOrDefault(h => h.GetType() == this.inputHandlerType);

            // TODO Check if InputHandler == null
            var result = await inputHandler
                .Execute(inputMessage, messageSlip)
                .ConfigureAwait(false);

            result.ExecutedHandlerName = this.inputHandlerType.Name;

            data.DataToPersist = result.DataToPersist ?? data.DataToPersist;
            foreach (var ouputMessageId in result.OutputMessagesIds)
            {
                data.PublishedMessageIdsResponseState.Add(ouputMessageId, Dtos.ResponseResultState.None);
            }

            data.State = result.State == Dtos.InputResultState.Successful
                ? Dtos.StepState.ExecutedInput
                : Dtos.StepState.FailedOnExecution;

            await this.dataRepository
                .Save(data)
                .ConfigureAwait(false);

            return result;
        }

        private async Task<ResponseResult> HandleResponse<T>(
            Type responseHandlerType,
            T successMessage,
            MessageSlip messageSlip)
            where T : class
        {
            var responseHandler = this.serviceProvider
                .GetServices<IHandlesResponse<T>>()
                .FirstOrDefault(h => h.GetType() == responseHandlerType);

            // TODO check if responsehandler is null

            var data = await this.dataRepository
                .LoadFromConversationId(this.StepName, messageSlip.ConversationId)
                .ConfigureAwait(false);

            if (IsStepWaitingResponses(data) && IsThisMessageNotProcessed(data, messageSlip))
            {
                var result = await responseHandler
                    .Execute(successMessage, messageSlip, data.DataToPersist)
                    .ConfigureAwait(false);

                result.ExecutedHandlerName = responseHandler.GetType().Name;
                UpdateDataAfterResponseExecution(data, result, messageSlip);
                return result;
            }
            else
            {
                // TODO - Log.
                return null;
            }
        }

        private bool IsStepWaitingResponses(Dtos.SapherStepData data)
            => data?.State == Dtos.StepState.None || data?.State == Dtos.StepState.ExecutedInput;

        private bool IsThisMessageNotProcessed(Dtos.SapherStepData data, MessageSlip messageSlip)
            => data.PublishedMessageIdsResponseState[messageSlip.ConversationId] == Dtos.ResponseResultState.None;

        private void UpdateDataAfterResponseExecution(
            Dtos.SapherStepData data,
            ResponseResult result,
            MessageSlip messageSlip)
        {
            data.DataToPersist = result.DataToPersist ?? data.DataToPersist;
            data.PublishedMessageIdsResponseState[messageSlip.ConversationId] = result.State;

            EvaluateStepState(data);
            this.dataRepository.Save(data);
        }

        private void EvaluateStepState(Dtos.SapherStepData data)
        {
            if (data.PublishedMessageIdsResponseState
                .All(responseState =>
                    responseState.Value != Dtos.ResponseResultState.None))
            {
                if (data.PublishedMessageIdsResponseState
                    .All(responseState =>
                        responseState.Value == Dtos.ResponseResultState.Successful))
                {
                    data.State = Dtos.StepState.Successful;
                }
                else if (data.PublishedMessageIdsResponseState
                    .Any(responseState =>
                        responseState.Value == Dtos.ResponseResultState.Failed))
                {
                    data.State = Dtos.StepState.FailedOnResponses;
                }
                else if (data.PublishedMessageIdsResponseState
                    .Any(responseState =>
                        responseState.Value == Dtos.ResponseResultState.Compensated))
                {
                    data.State = Dtos.StepState.Compensated;
                }
            }

            // TODO Develop Background job that checks if the Step never had all the expected answers
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