namespace Sapher.Logger
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// LogLevels to be used by <see cref="LogEntry"/>
    /// </summary>
    public enum LoggingEventType
    {
        /// <summary>
        /// Verbose Log Level for <see cref="LogEntry"/>
        /// </summary>
        Verbose,

        /// <summary>
        /// Information Log Level for <see cref="LogEntry"/>
        /// </summary>
        Information,

        /// <summary>
        /// Warning Log Level for <see cref="LogEntry"/>
        /// </summary>
        Warning,

        /// <summary>
        /// Error Log Level for <see cref="LogEntry"/>
        /// </summary>
        Error,

        /// <summary>
        /// Fatal Log Level for <see cref="LogEntry"/>
        /// </summary>
        Fatal
    }

    /// <summary>
    /// LogEntry contains information regarding a specific Log.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// The severity of this log.
        /// Please check <see cref="LoggingEventType"/>
        /// </summary>
        public LoggingEventType Severity { get; }

        /// <summary>
        /// The message to log.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The exception to log
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Additional data to log
        /// </summary>
        public IDictionary<string, string> AdditionalData { get; }

        /// <summary>
        /// LogEntry constructor. Creates a new instance of LogEntry with the provided parameters.
        /// </summary>
        /// <param name="severity">The severity of the LogEntry</param>
        /// <param name="message">The message of the LogEntry</param>
        /// <param name="exception">The exception of the LogEntry. This is optional.</param>
        /// <param name="additionalData">Additional data to add to the LogEntry. This is optional.</param>
        /// <exception cref = "System.ArgumentNullException"><paramref name="message"/> is null</exception> 
        /// <exception cref = "System.ArgumentException"><paramref name="message"/> is empty or whitespace</exception> 
        public LogEntry(
            LoggingEventType severity,
            string message,
            Exception exception = null,
            IDictionary<string, string> additionalData = null)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message?.Trim().Length == 0) throw new ArgumentException("empty or whitespace", nameof(message));

            this.Severity = severity;
            this.Message = message;
            this.Exception = exception;
            this.AdditionalData = additionalData;
        }
    }
}