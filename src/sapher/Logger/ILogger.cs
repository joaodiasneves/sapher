namespace Sapher.Logger
{
    /// <summary>
    /// Sapher Logging Interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs the provided LogEntry information
        /// </summary>
        /// <param name="entry">Log information to be logged by the Logging implementation</param>
        void Log(LogEntry entry);
    }
}