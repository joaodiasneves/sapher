namespace Sapher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Dtos;
    using Handlers;
    using Persistence;
    using TypeAdapters;
    using Model = Persistence.Model;

    public partial class SapherStep : ISapherStep
    {
        // TODO - Provide a Job executing in the background to check for Steps that never received all the answers
        public string StepName { get; set; }

        private readonly ISapherStepConfiguration configuration;
        private Type inputMessageType;
        private IHandlesInput inputHandler;
        private IDictionary<Type, IHandlesResponse> responseHandlers = new Dictionary<Type, IHandlesResponse>();
        private ISapherDataRepository dataRepository;

        internal SapherStep(ISapherStepConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Deliver<T>(T message, MessageSlip messageSlip) where T : class
        {
            // TODO  add logs.
            // TODO  trycatches

            var messageType = typeof(T);

            if (messageType == this.inputMessageType)
            {
                await this.HandleExecution(message, messageSlip).ConfigureAwait(false);
            }
            else if (this.responseHandlers.TryGetValue(messageType, out var responseHandler))
            {
                var responseHandlerCasted = (IHandlesResponse<T>)responseHandler;
                await this.HandleResponse(responseHandlerCasted, message, messageSlip).ConfigureAwait(false);
            }
        }

        private async Task HandleExecution<T>(
            T inputMessage,
            MessageSlip messageSlip)
            where T : class
        {
            // Idempotency
            // TODO ensure versioning
            var data = this.dataRepository.Load(
                this.StepName,
                messageSlip.MessageId);

            if (data != null)
            {
                await Task.CompletedTask; // TODO LOG
                return;
            }

            data = new Model.SapherStepData(messageSlip.ToDataModel(), this.StepName);

            var handler = (IHandlesInput<T>)this.inputHandler;
            var result = await handler
                .Execute(inputMessage)
                .ConfigureAwait(false);

            data.DataToPersist = result.DataToPersist;
            foreach (var ouputMessageId in result.OutputMessagesIds)
            {
                data.PublishedMessageIdsResponseState.Add(ouputMessageId, Model.ResponseResultState.None);
            }

            data.State = result.State == InputResultState.Successful
                ? Model.StepState.ExecutedInput
                : Model.StepState.FailedOnExecution;

            this.dataRepository.Save(data);
        }

        private async Task HandleResponse<T>(
            IHandlesResponse<T> responseHandler,
            T successMessage,
            MessageSlip messageSlip)
            where T : class
        {
            if (IsStepWaitingResponses(messageSlip, out var data)
                && IsThisMessageNotProcessed(data, messageSlip))
            {
                var result = await responseHandler
                    .Execute(successMessage, data)
                    .ConfigureAwait(false);

                UpdateData(data, result, messageSlip);
            }
        }

        private bool IsStepWaitingResponses(MessageSlip messageSlip, out Model.SapherStepData data)
        {
            data = this.dataRepository.LoadFromConversationId(
               this.StepName,
               messageSlip.ConversationId);

            return data?.State == Model.StepState.None || data?.State == Model.StepState.ExecutedInput;
        }

        private bool IsThisMessageNotProcessed(Model.SapherStepData data, MessageSlip messageSlip)
            => data.PublishedMessageIdsResponseState[messageSlip.ConversationId] == Model.ResponseResultState.None;

        private void UpdateData(
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
    }
}