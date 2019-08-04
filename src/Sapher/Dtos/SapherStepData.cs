namespace Sapher.Dtos
{
    using System.Collections.Generic;

    public class SapherStepData
    {
        public SapherStepData(MessageSlip messageSlip, string stepName)
        {
            this.StepName = string.IsNullOrWhiteSpace(stepName)
                ? this.GetType().UnderlyingSystemType.Name
                : stepName;

            this.InputMessageSlip = messageSlip;

            this.PublishedMessageIdsResponseState = new Dictionary<string, ResponseResultState>();
        }

        public SapherStepData()
        {
        }

        public string StepName { get; set; }

        public MessageSlip InputMessageSlip { get; set; }

        public StepState State { get; set; }

        public IDictionary<string, ResponseResultState> PublishedMessageIdsResponseState { get; set; }

        public IDictionary<string, string> DataToPersist { get; set; }
    }
}