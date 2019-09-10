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

        internal IDictionary<string, ResponseResultState> PublishedMessageIdsResponseState { get; set; }

        internal IDictionary<string, string> DataToPersist { get; set; }

        internal DateTime CreationDate { get; set; }

        internal DateTime UpdatedOn { get; set; }

        internal SapherStepData()
        {
        }

        internal SapherStepData(
            MessageSlip inputMessageSlip,
            string stepName = null)
        {
            this.StepName = string.IsNullOrWhiteSpace(stepName)
                ? this.GetType().UnderlyingSystemType.Name
                : stepName;

            this.InputMessageSlip = inputMessageSlip;

            this.PublishedMessageIdsResponseState = new Dictionary<string, ResponseResultState>();
        }

        internal static string GenerateId(string stepName, string messageId)
            => $"{stepName}-{messageId}";
    }
}