namespace Sapher.Dtos
{
    using System.Collections.Generic;

    public class SapherStepData
    {
        public string StepName { get; set; }

        public MessageSlip InputMessageSlip { get; set; }

        public StepState State { get; set; }

        public IDictionary<string, ResponseResultState> PublishedMessageIdsResponseState { get; set; }

        public IDictionary<string, string> DataToPersist { get; set; }
    }
}