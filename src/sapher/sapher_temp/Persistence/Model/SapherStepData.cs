namespace Sapher.Persistence.Model
{
    using System;
    using System.Collections.Generic;

    internal class SapherStepData
    {
        internal string Id => GenerateId(this.StepName, this.InputMessageSlip.MessageId);

        internal string StepName { get; set; }

        internal MessageSlip InputMessageSlip { get; set; }

        internal StepState State { get; set; }

        internal IList<string> MessagesWaitingResponse { get; set; }

        internal IList<string> SuccessfulMessages { get; set; }

        internal IList<string> FailedMessages { get; set; }

        internal IList<string> CompensatedMessages { get; set; }

        internal IDictionary<string, string> DataToPersist { get; set; }

        internal DateTime CreationDate { get; set; }

        internal DateTime UpdatedOn { get; set; }

        internal SapherStepData()
        {
        }

        //internal SapherStepData(
        //    MessageSlip inputMessageSlip,
        //    string stepName = null)
        //{
        //    this.StepName = string.IsNullOrWhiteSpace(stepName)
        //        ? this.GetType().UnderlyingSystemType.Name
        //        : stepName;

        //    this.InputMessageSlip = inputMessageSlip;

        //    this.MessagesWaitingResponse = new List<string>();
        //    this.SuccessfulMessages = new List<string>();
        //    this.FailedMessages = new List<string>();
        //    this.CompensatedMessages = new List<string>();

        //    this.DataToPersist = new Dictionary<string, string>();
        //}

        internal static string GenerateId(string stepName, string messageId)
            => $"{stepName}-{messageId}";
    }
}