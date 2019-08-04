namespace Sapher.Logger
{
    using System;

    public enum LoggingEventType { Debug, Information, Warning, Error, Fatal };

    public class LogEntry
    {
        public readonly LoggingEventType Severity;
        public readonly string Message;
        public readonly Exception Exception;

        public LogEntry(LoggingEventType severity, string message, Exception exception = null)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message?.Length == 0) throw new ArgumentException("empty", nameof(message));

            this.Severity = severity;
            this.Message = message;
            this.Exception = exception;
        }
    }
}