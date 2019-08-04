namespace Sapher.Logger.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class LoggerExtensions
    {
        public static void Verbose(this ILogger logger, string message, params KeyValuePair<string, string>[] additionalData)
        {
            var additionalDataDic = additionalData.ToDictionary(k => k.Key, v => v.Value);
            logger.Log(new LogEntry(LoggingEventType.Verbose, message, additionalData: additionalDataDic));
        }

        public static void Verbose(this ILogger logger, string message, Dictionary<string, string> additionalData = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Verbose, message, additionalData: additionalData));
        }

        public static void Info(this ILogger logger, string message, params KeyValuePair<string, string>[] additionalData)
        {
            var additionalDataDic = additionalData.ToDictionary(k => k.Key, v => v.Value);
            logger.Log(new LogEntry(LoggingEventType.Information, message, additionalData: additionalDataDic));
        }

        public static void Info(this ILogger logger, string message, Dictionary<string, string> additionalData = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message, additionalData: additionalData));
        }

        public static void Warning(this ILogger logger, string message, params KeyValuePair<string, string>[] additionalData)
        {
            var additionalDataDic = additionalData.ToDictionary(k => k.Key, v => v.Value);
            logger.Log(new LogEntry(LoggingEventType.Warning, message, additionalData: additionalDataDic));
        }

        public static void Warning(this ILogger logger, string message, Dictionary<string, string> additionalData = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, message, additionalData: additionalData));
        }

        public static void Warning(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(
                LoggingEventType.Warning,
                exception.Message,
                exception,
                exception.Data as IDictionary<string, string>));
        }

        public static void Error(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(
                LoggingEventType.Error,
                exception.Message,
                exception,
                exception.Data as IDictionary<string, string>));
        }
    }
}