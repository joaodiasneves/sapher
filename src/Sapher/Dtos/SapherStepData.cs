namespace Sapher.Dtos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SapherStepData
    {
        public string StepName { get; set; }

        public MessageSlip InputMessageSlip { get; set; }

        public StepState State { get; set; }

        public IList<string> MessagesWaitingResponse { get; set; }

        public IList<string> SuccessfulMessages { get; set; }

        public IList<string> FailedMessages { get; set; }

        public IList<string> CompensatedMessages { get; set; }

        public IDictionary<string, string> DataToPersist { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime UpdateDate { get; set; }

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
            => this.State != Dtos.StepState.None
            && this.State != Dtos.StepState.ExecutedInput
            && this.State != Dtos.StepState.Timeout;

        internal bool IsExpectingResponses
            => this.MessagesWaitingResponse?.Count > 0;

        internal bool IsMessageAlreadyProcessed(string messageId)
            => !this.MessagesWaitingResponse.Contains(messageId);

        private void Init()
        {
            this.MessagesWaitingResponse = new List<string>();
            this.SuccessfulMessages = new List<string>();
            this.FailedMessages = new List<string>();
            this.CompensatedMessages = new List<string>();
            this.DataToPersist = new Dictionary<string, string>();
            this.CreationDate = DateTime.UtcNow;
            this.UpdateDate = this.CreationDate;
        }
    }
}