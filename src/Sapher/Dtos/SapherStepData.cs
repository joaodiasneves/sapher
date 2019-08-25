namespace Sapher.Dtos
{
    using System;
    using System.Collections.Generic;

    public class SapherStepData
    {
        public string StepName { get; set; }

        public MessageSlip InputMessageSlip { get; set; }

        public StepState State { get; set; }

        public IDictionary<string, ResponseResultState> PublishedMessageIdsResponseState { get; set; }

        public IDictionary<string, string> DataToPersist { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime UpdatedOn { get; set; }

        public SapherStepData(MessageSlip messageSlip, string stepName)
        {
            this.StepName = string.IsNullOrWhiteSpace(stepName)
                ? this.GetType().UnderlyingSystemType.Name
                : stepName;

            this.InputMessageSlip = messageSlip;

            this.Init();
        }

        public SapherStepData()
        {
            this.Init();
        }

        internal bool IsFinished
            => this.State != Dtos.StepState.None && this.State != Dtos.StepState.ExecutedInput;

        internal bool IsExpectingResponses
            => this.PublishedMessageIdsResponseState?.Count > 0;

        internal bool IsMessageAlreadyProcessed(MessageSlip messageSlip)
            => this.PublishedMessageIdsResponseState[messageSlip.ConversationId] != Dtos.ResponseResultState.None;

        private void Init()
        {
            this.PublishedMessageIdsResponseState = new Dictionary<string, ResponseResultState>();
            this.CreationDate = DateTime.UtcNow;
            this.UpdatedOn = this.CreationDate;
        }
    }
}