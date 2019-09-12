namespace Sapher.Logger
{
    internal class NullLogger : ILogger
    {
        public void Log(LogEntry entry)
        {
            // Empty implementation to log nothing
        }
    }
}