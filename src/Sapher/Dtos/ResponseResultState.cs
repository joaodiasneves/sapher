namespace Sapher.Dtos
{
    /// <summary>
    /// Possible Response Message Handler Execution's result states
    /// </summary>
    public enum ResponseResultState
    {
        /// <summary>
        /// Default value for ResponseResultState
        /// </summary>
        None = 0,

        /// <summary>
        /// Defines a Successful response handler execution
        /// </summary>
        Successful = 1,

        /// <summary>
        /// Defines a Compensated response handler execution
        /// </summary>
        Compensated = 2,

        /// <summary>
        /// Defines a Failed response handler execution
        /// </summary>
        Failed = 3
    }
}