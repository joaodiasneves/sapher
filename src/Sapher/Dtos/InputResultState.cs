namespace Sapher.Dtos
{
    /// <summary>
    /// Possible Input Message Execution's result states
    /// </summary>
    public enum InputResultState
    {
        /// <summary>
        /// Default value for InputResultState
        /// </summary>
        None = 0,

        /// <summary>
        /// Defines a successful input execution
        /// </summary>
        Successful = 1,

        /// <summary>
        /// Defines a failed input execution
        /// </summary>
        Failed = 2
    }
}