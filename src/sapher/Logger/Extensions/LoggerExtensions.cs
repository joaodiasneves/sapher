namespace Sapher.Logger.Extensions
{
    using System;

    public static class LoggerExtensions
    {
        public static void Info(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message));
        }

        public static void Warning(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, message));
        }

        public static void Warning(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, exception.Message, exception));
        }

        public static void Error(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, exception.Message, exception));
        }
    }
}