using System;

namespace Test.It.With.Amqp.Logging
{
    internal sealed class LogMessage
    {
        public LogMessage(string loggerName, LogLevel logLevel, string template, object[] arguments,
            Exception exception = null)
        {
            LoggerName = loggerName;
            LogLevel = logLevel;
            Template = template;
            Arguments = arguments;
            Exception = exception;
        }

        public string LoggerName { get; }
        public LogLevel LogLevel { get; }
        public string Template { get; }
        public object[] Arguments { get; }
        public Exception Exception { get; }
    }
}