namespace Sapher.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Logger;

    [ExcludeFromCodeCoverage]
    internal class TestLogger : ILogger
    {
        public static LogEntry ReceivedLogEntry { get; set; }

        public void Log(LogEntry entry)
        {
            ReceivedLogEntry = entry;
        }
    }
}