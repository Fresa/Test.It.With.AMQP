using System;
using NLog;
using LogLevel = NLog.LogLevel;

namespace Test.It.With.Amqp.Tests.Logging
{
    internal static class NLogLogger
    {
        public static void Log(string loggerName, Amqp.Logging.LogLevel logLevel, string template, object[] args, Exception exception = null)
        {
            switch (logLevel)
            {
                case Amqp.Logging.LogLevel.Fatal:
                    Log(LogLevel.Fatal, loggerName, template, args, exception);
                    break;
                case Amqp.Logging.LogLevel.Error:
                    Log(LogLevel.Error, loggerName, template, args, exception);
                    break;
                case Amqp.Logging.LogLevel.Warning:
                    Log(LogLevel.Warn, loggerName, template, args, exception);
                    break;
                case Amqp.Logging.LogLevel.Info:
                    Log(LogLevel.Info, loggerName, template, args, exception);
                    break;
                case Amqp.Logging.LogLevel.Debug:
                    Log(LogLevel.Debug, loggerName, template, args, exception);
                    break;
                case Amqp.Logging.LogLevel.Trace:
                    Log(LogLevel.Trace, loggerName, template, args, exception);
                    break;
            }
        }

        private static void Log(LogLevel logLevel, string loggerName, string template, object[] args, Exception ex = null)
        {
            var logger = LogManager.GetLogger(loggerName);
            if (!logger.IsEnabled(logLevel))
            {
                return;
            }

            UpdateLogicalThreadContexts();
            logger.Log(logLevel, ex, template, args);
        }

        private static void UpdateLogicalThreadContexts()
        {
            MappedDiagnosticsLogicalContext.Clear(false);
            foreach (var (key, value) in Amqp.Logging.Logger.GetLogicalThreadContexts())
            {
                MappedDiagnosticsLogicalContext.Set(key, value);
            }
        }
    }
}