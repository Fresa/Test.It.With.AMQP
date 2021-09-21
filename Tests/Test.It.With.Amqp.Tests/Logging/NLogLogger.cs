using System;
using NLog;
using Logger = Test.It.With.Amqp.Logging.Logger;
using LogLevel = NLog.LogLevel;

namespace Test.It.With.Amqp.Tests.Logging
{
    internal sealed class NLogLogger : Logger
    {
        public override void Fatal(string loggerName, string template, object[] args, Exception ex = null)
        {
            Log(LogLevel.Fatal, loggerName, template, args);
        }

        public override void Error(string loggerName, string template, object[] args, Exception ex = null)
        {
            Log(LogLevel.Error, loggerName, template, args);
        }

        public override void Warning(string loggerName, string template, object[] args, Exception ex = null)
        {
            Log(LogLevel.Warn, loggerName, template, args);
        }

        public override void Info(string loggerName, string template, object[] args, Exception ex = null)
        {
            Log(LogLevel.Info, loggerName, template, args);
        }

        public override void Debug(string loggerName, string template, object[] args, Exception ex = null)
        {
            Log(LogLevel.Debug, loggerName, template, args);
        }

        public override void Trace(string loggerName, string template, object[] args, Exception ex = null)
        {
            Log(LogLevel.Trace, loggerName, template, args);
        }

        private void Log(LogLevel logLevel, string loggerName, string template, object[] args, Exception ex = null)
        {
            var logger = LogManager.GetLogger(loggerName);
            if (!logger.IsEnabled(logLevel))
            {
                return;
            }

            UpdateLogicalThreadContexts();
            logger.Log(logLevel, ex, template, args);
        }

        private void UpdateLogicalThreadContexts()
        {
            MappedDiagnosticsLogicalContext.Clear(false);
            foreach (var logicalThreadContext in GetLogicalThreadContexts())
            {
                MappedDiagnosticsLogicalContext.Set(logicalThreadContext.Key, logicalThreadContext.Value);
            }
        }
    }
}