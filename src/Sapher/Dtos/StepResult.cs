namespace Sapher.Dtos
{
    /// <summary>
    /// DTO to provide information regarding the result of a Step execution
    /// </summary>
    public class StepResult
    {
        /// <summary>
        /// The name of the executed Step
        /// </summary>
        public string StepName { get; }

        /// <summary>
        /// The result of the Input Handler, if it was executed. Otherwise, it will be null.
        /// </summary>
        public InputResult InputHandlerResult { get; set; }

        /// <summary>
        /// The result of the Response Handler, if it was executed. Otherwise, it will be null.
        /// </summary>
        public ResponseResult ResponseHandlerResult { get; set; }

        /// <summary>
        /// A flag indicating if the step execution failed or not.
        /// </summary>
        public bool IsStepFailed { get; set; }

        /// <summary>
        /// Provides information regarding the error occurred if <c>IsStepFailed == true</c>. Null if the step succeeds.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Initializes <see cref="StepResult"/> using the defined <paramref name="stepName"/>
        /// </summary>
        /// <param name="stepName">The name of the step to initialize</param>
        public StepResult(string stepName)
        {
            this.StepName = stepName;
        }
    }
}