namespace Sapher.Dtos
{
    /// <summary>
    /// Possible Step Execution's result states
    /// </summary>
    public enum StepState
    {
        /// <summary>
        /// Default value for <see cref="StepState"/>
        /// </summary>
        None = 0,

        /// <summary>
        /// Step input executed successfully, Step is waiting for responses.
        /// </summary>
        ExecutedInput = 1,

        /// <summary>
        /// Step executed successfully. It is not waiting for responses.
        /// </summary>
        Successful = 2,

        /// <summary>
        /// Step execution was compensated on at least one response.
        /// </summary>
        Compensated = 3,

        /// <summary>
        /// Step input execution failed
        /// </summary>
        FailedOnExecution = 4,

        /// <summary>
        /// Step response handling failed on at least one response
        /// </summary>
        FailedOnResponses = 5,

        /// <summary>
        /// Step timedout before receiveing all the required responses
        /// </summary>
        Timeout = 6
    }
}