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
    using TypeAdapters;
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

            data = new Model.SapherStepData(messageSlip.ToDataModel(), this.StepName);

            var inputHandler = this.serviceProvider
                    .GetServices<IHandlesInput<T>>()
                    .First(h => h.GetType() == this.inputHandlerType);
            // TODO Check if InputHandler == null
            var result = await inputHandler
                .Execute(inputMessage, messageSlip)
                .ConfigureAwait(false);

            result.ExecutedHandlerName = this.inputHandlerType.Name;

            data.DataToPersist = result.DataToPersist ?? data.DataToPersist;
            foreach (var ouputMessageId in result.OutputMessagesIds)
            {
                data.PublishedMessageIdsResponseState.Add(ouputMessageId, Model.ResponseResultState.None);
            }

            data.State = result.State == InputResultState.Successful
                ? Model.StepState.ExecutedInput
                : Model.StepState.FailedOnExecution;

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
                   .First(h => h.GetType() == responseHandlerType);

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

        private bool IsStepWaitingResponses(Model.SapherStepData data)
            => data?.State == Model.StepState.None || data?.State == Model.StepState.ExecutedInput;

        private bool IsThisMessageNotProcessed(Model.SapherStepData data, MessageSlip messageSlip)
            => data.PublishedMessageIdsResponseState[messageSlip.ConversationId] == Model.ResponseResultState.None;

        private void UpdateDataAfterResponseExecution(
            Model.SapherStepData data,
            ResponseResult result,
            MessageSlip messageSlip)
        {
            data.DataToPersist = result.DataToPersist ?? data.DataToPersist;
            data.PublishedMessageIdsResponseState[messageSlip.ConversationId] = result.State.ToDataModel();

            EvaluateStepState(data);
            this.dataRepository.Save(data);
        }

        private void EvaluateStepState(Model.SapherStepData data)
        {
            if (data.PublishedMessageIdsResponseState
                .All(responseState =>
                    responseState.Value != Model.ResponseResultState.None))
            {
                if (data.PublishedMessageIdsResponseState
                    .All(responseState =>
                        responseState.Value == Model.ResponseResultState.Successful))
                {
                    data.State = Model.StepState.Successful;
                }
                else if (data.PublishedMessageIdsResponseState
                    .Any(responseState =>
                        responseState.Value == Model.ResponseResultState.Failed))
                {
                    data.State = Model.StepState.FailedOnResponses;
                }
                else if (data.PublishedMessageIdsResponseState
                    .Any(responseState =>
                        responseState.Value == Model.ResponseResultState.Compensated))
                {
                    data.State = Model.StepState.Compensated;
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