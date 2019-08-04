namespace Sapher.Dtos
{
    using System;
    using System.Collections.Generic;

    public class DeliveryResult
    {
        public bool IsDeliveryFailed { get; set; }

        public string ErrorMessage { get; set; }

        public Exception Exception { get; set; }

        public IEnumerable<StepResult> StepsExecuted { get; set; }

        public DeliveryResult()
        {
            this.StepsExecuted = new List<StepResult>();
        }
    }
}