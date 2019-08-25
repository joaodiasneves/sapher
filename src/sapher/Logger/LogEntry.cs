    namespace Sapher.Logger
{
    using System;
    using System.Collections.Generic;

    public enum LoggingEventType { Verbose, Information, Warning, Error, Fatal }

    public class LogEntry
    {
        public LoggingEventType Severity { get; }

        public string Message { get; }

        public Exception Exception { get; }

        public IDictionary<string, string> AdditionalData { get; }

        public LogEntry(LoggingEventType severity, string message, Exception exception = null, IDictionary<string, string> additionalData = null)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message?.Length == 0) throw new ArgumentException("empty", nameof(message));

            this.Severity = severity;
            this.Message = message;
            this.Exception = exception;
            this.AdditionalData = additionalData;
        }
    }
}