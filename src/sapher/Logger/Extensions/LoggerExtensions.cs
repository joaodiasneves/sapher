namespace Sapher.Logger.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class LoggerExtensions
    {
        internal static void Verbose(this ILogger logger, string message, params KeyValuePair<string, string>[] additionalData)
        {
            var additionalDataDic = additionalData.ToDictionary(k => k.Key, v => v.Value);
            logger.Log(new LogEntry(LoggingEventType.Verbose, message, additionalData: additionalDataDic));
        }

        internal static void Verbose(this ILogger logger, string message, Dictionary<string, string> additionalData = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Verbose, message, additionalData: additionalData));
        }

        internal static void Info(this ILogger logger, string message, params KeyValuePair<string, string>[] additionalData)
        {
            var additionalDataDic = additionalData.ToDictionary(k => k.Key, v => v.Value);
            logger.Log(new LogEntry(LoggingEventType.Information, message, additionalData: additionalDataDic));
        }

        internal static void Info(this ILogger logger, string message, Dictionary<string, string> additionalData = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message, additionalData: additionalData));
        }

        internal static void Warning(this ILogger logger, string message, params KeyValuePair<string, string>[] additionalData)
        {
            var additionalDataDic = additionalData.ToDictionary(k => k.Key, v => v.Value);
            logger.Log(new LogEntry(LoggingEventType.Warning, message, additionalData: additionalDataDic));
        }

        internal static void Warning(this ILogger logger, string message, Dictionary<string, string> additionalData = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, message, additionalData: additionalData));
        }

        internal static void Warning(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(
                LoggingEventType.Warning,
                exception.Message,
                exception,
                exception.Data as IDictionary<string, string>));
        }

        internal static void Error(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(
                LoggingEventType.Error,
                exception.Message,
                exception,
                exception.Data as IDictionary<string, string>));
        }
    }
}