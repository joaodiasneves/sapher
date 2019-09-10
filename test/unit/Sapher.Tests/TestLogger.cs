namespace Sapher.Tests
{
    using Logger;

    internal class TestLogger : ILogger
    {
        public LogEntry ReceivedLogEntry{ get; set; }

        public void Log(LogEntry entry)
        {
            this.ReceivedLogEntry = entry;
        }
    }
}