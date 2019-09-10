namespace Sapher.Dtos
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an Instance of a SapherStep and all its data.
    /// </summary>
    public class SapherStepData
    {
        /// <summary>
        /// The name of the Step
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// The <see cref="MessageSlip"></see> of the input message
        /// </summary>
        public MessageSlip InputMessageSlip { get; set; }

        /// <summary>
        /// The current State of the Step execution
        /// </summary>
        public StepState State { get; set; }

        /// <summary>
        /// Contains the Message Ids of all the messages published or sent by the Step's Input Handler execution
        /// </summary>
        public IList<string> MessagesWaitingResponse { get; set; }

        /// <summary>
        /// Contains the Message Ids of all the messages published or sent by the Step's Input Handler execution
        /// that have received successful responses.
        /// </summary>
        public IList<string> SuccessfulMessages { get; set; }

        /// <summary>
        /// Contains the Message Ids of all the messages published or sent by the Step's Input Handler execution
        /// that have received failed responses.
        /// </summary>
        public IList<string> FailedMessages { get; set; }

        /// <summary>
        /// Contains the Message Ids of all the messages published or sent by the Step's Input Handler execution
        /// that have received compensated responses.
        /// </summary>
        public IList<string> CompensatedMessages { get; set; }

        /// <summary>
        /// Data to persist, provided by the Step's Input Handler result. Should be used for handling responses (for instance, compensation logic).
        /// </summary>
        public IDictionary<string, string> DataToPersist { get; set; }

        /// <summary>
        /// The creation date of this Step instance.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The date this step instance was last updated.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Initializes a new SapherStepData instance using the provided <paramref name="messageSlip"/> and <paramref name="stepName"/>
        /// </summary>
        /// <param name="messageSlip">The messageSlip to be used for the initialization of the instance. Usually is the input message's message slip</param>
        /// <param name="stepName">The name of the Step</param>
        public SapherStepData(MessageSlip messageSlip, string stepName)
        {
            this.StepName = string.IsNullOrWhiteSpace(stepName)
                ? this.GetType().UnderlyingSystemType.Name
                : stepName;

            this.InputMessageSlip = messageSlip;

            this.Init();
        }

        /// <summary>
        /// Initializes a new SapherStepData instance
        /// </summary>
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