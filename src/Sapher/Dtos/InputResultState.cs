namespace Sapher.Dtos
{
    /// <summary>
    /// Possible Input Message Handler Execution's result states
    /// </summary>
    public enum InputResultState
    {
        /// <summary>
        /// Default value for InputResultState
        /// </summary>
        None = 0,

        /// <summary>
        /// Defines a successful input handler execution
        /// </summary>
        Successful = 1,

        /// <summary>
        /// Defines a failed input handler execution
        /// </summary>
        Failed = 2
    }
}