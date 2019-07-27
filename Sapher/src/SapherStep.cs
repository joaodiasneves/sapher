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
    using Persistence.Model;

    public partial class SapherStep : ISapherStep
    {
        // TODO - Simplify code in this class
        // TODO - Provide a Job executing in the background to check for Steps that never received all the answers
        public string StepName { get; set; }

        private readonly ISapherStepConfiguration configuration;
        private Type inputMessageType;
        private IHandlesStepInput inputHandler;
        private IDictionary<Type, IHandlesSuccess> successHandlers = new Dictionary<Type, IHandlesSuccess>();
        private IDictionary<Type, IHandlesCompensation> compensationHandlers = new Dictionary<Type, IHandlesCompensation>();
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
                await this.HandleExecution<T>(message, messageSlip).ConfigureAwait(false);
            }
            else if (this.successHandlers.TryGetValue(messageType, out var successHandler))
            {
                var successHandlerCasted = (IHandlesSuccess<T>)successHandler;
                await this.HandleSuccess<T>(successHandlerCasted, message, messageSlip).ConfigureAwait(false);
            }
            else if (this.compensationHandlers.TryGetValue(messageType, out var compensationHandler))
            {
                var compensationHandlerCasted = (IHandlesCompensation<T>)compensationHandler;
                await this.HandleCompensation<T>(compensationHandlerCasted, message, messageSlip).ConfigureAwait(false);
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

            data = new SapherStepData(messageSlip, this.StepName);

            var handler = (IHandlesStepInput<T>)this.inputHandler;
            var result = await handler
                .Execute(inputMessage)
                .ConfigureAwait(false);

            data.DataToPersist = result.DataToPersist;
            foreach (var ouputMessageId in result.OutputMessagesIds)
            {
                data.OutputMessageIdsState.Add(ouputMessageId, OutputState.None);
            }

            data.State = result.IsSuccess
                ? StepState.Executed
                : StepState.FailedOnExecution;

            this.dataRepository.Save(data);
        }

        private async Task HandleSuccess<T>(
            IHandlesSuccess<T> successHandler,
            T successMessage,
            MessageSlip messageSlip)
            where T : class
        {
            if (IsStepWaitingResponses(messageSlip, out var data)
                && IsThisMessageNotProcessed(data, messageSlip))
            {
                var result = await successHandler
                    .Execute(successMessage, data)
                    .ConfigureAwait(false);

                UpdateData(
                    data,
                    result,
                    messageSlip,
                    result.IsSuccess
                        ? OutputState.Successful
                        : OutputState.FailedOnSuccess);
            }
        }

        private async Task HandleCompensation<T>(
           IHandlesCompensation<T> compensationHandler,
           T compensationMessage,
           MessageSlip messageSlip)
           where T : class
        {
            if (IsStepWaitingResponses(messageSlip, out var data)
                && IsThisMessageNotProcessed(data, messageSlip))
            {
                var result = await compensationHandler
                    .Compensate(compensationMessage, data)
                    .ConfigureAwait(false);

                UpdateData(
                    data,
                    result,
                    messageSlip,
                    result.IsSuccess
                        ? OutputState.Compensated
                        : OutputState.FailedOnCompensation);
            }
        }

        private bool IsStepWaitingResponses(MessageSlip messageSlip, out SapherStepData data)
        {
            data = this.dataRepository.LoadFromConversationId(
               this.StepName,
               messageSlip.ConversationId);

            if (data == null || (data.State != StepState.None && data.State != StepState.Executed))
            {
                // TODO LOG - it is invalid ConversationId or already processed
                return false;
            }

            return true;
        }

        private bool IsThisMessageNotProcessed(SapherStepData data, MessageSlip messageSlip)
        {
            if (data.OutputMessageIdsState[messageSlip.ConversationId] != OutputState.None)
            {
                // The answer to this output was already received... 
                // TODO LOG
                return false;
            }

            return true;
        }

        private void UpdateData(
            SapherStepData data,
            Result result,
            MessageSlip messageSlip,
            OutputState outputState)
        {
            data.DataToPersist = result.DataToPersist;
            data.OutputMessageIdsState[messageSlip.ConversationId] = outputState;

            EvaluateStepState(data);
            this.dataRepository.Save(data);
        }

        private void EvaluateStepState(SapherStepData data)
        {
            if (data.OutputMessageIdsState
                .All(outputState => outputState.Value != OutputState.None))
            {
                if (data.OutputMessageIdsState
                    .All(outputState => outputState.Value == OutputState.Successful))
                {
                    data.State = StepState.Successful;
                }
                else if (data.OutputMessageIdsState
                    .All(outputState => outputState.Value == OutputState.Compensated))
                {
                    data.State = StepState.Compensated;
                }
                else if (data.OutputMessageIdsState
                    .All(outputState => outputState.Value == OutputState.FailedOnCompensation))
                {
                    data.State = StepState.FailedOnCompensation;
                }
                else if (data.OutputMessageIdsState
                    .All(outputState => outputState.Value == OutputState.FailedOnSuccess))
                {
                    data.State = StepState.FailedOnSuccess;
                }
                else 
                {
                    data.State = StepState.MultipleStates;
                }
            }
        }
    }
}