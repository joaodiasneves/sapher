namespace Sapher.Dtos
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// DTO to provide information regarding the result of a message delivery execution
    /// </summary>
    public class DeliveryResult
    {

        /// <summary>
        /// True if the delivery fails. In that case, ErrorMessage and Exception may provide more information regarding the cause.
        /// </summary>
        public bool IsDeliveryFailed { get; set; }

        /// <summary>
        /// An error message providing more information regarding the delivery failure.
        /// </summary>
        public string ErrorMessage { get; set; }

        
        /// <summary>
        /// An exception object providing more information regarding the delivery failure.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Identified SapherSteps' execution results.
        /// </summary>
        public IEnumerable<StepResult> StepsExecuted { get; set; }


        /// <summary>
        /// DeliveryResult class constructor
        /// </summary>
        public DeliveryResult()
        {
            this.StepsExecuted = new List<StepResult>();
        }
    }
}