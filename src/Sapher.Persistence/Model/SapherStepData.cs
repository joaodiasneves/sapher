namespace Sapher.Persistence.Model
{
    using System.Collections.Generic;

    public class SapherStepData
    {
        public string Id => GenerateId(this.StepName, this.InputMessageSlip.MessageId);

        public string StepName { get; set; }

        public MessageSlip InputMessageSlip { get; set; }

        public StepState State { get; set; }

        public IDictionary<string, ResponseResultState> PublishedMessageIdsResponseState { get; set; }

        public IDictionary<string, string> DataToPersist { get; set; }

        public SapherStepData()
        {
        }

        public SapherStepData(
            MessageSlip inputMessageSlip,
            string stepName = null)
        {
            this.StepName = string.IsNullOrWhiteSpace(stepName)
                ? this.GetType().UnderlyingSystemType.Name
                : stepName;

            this.InputMessageSlip = inputMessageSlip;

            this.PublishedMessageIdsResponseState = new Dictionary<string, ResponseResultState>();
        }

        public static string GenerateId(string stepName, string messageId)
            => $"{stepName}-{messageId}";
    }
}