namespace Sapher.Dtos
{
    using System.Collections.Generic;

    public class DeliveryResult
    {
        public IEnumerable<StepResult> StepsExecuted { get; set; }

        public DeliveryResult()
        {
            this.StepsExecuted = new List<StepResult>();
        }
    }
}