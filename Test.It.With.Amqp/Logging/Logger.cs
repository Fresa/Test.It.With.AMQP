using System;
using System.Collections.Generic;
using Test.It.With.Amqp.Logging.Annotations;

namespace Test.It.With.Amqp.Logging
{
    public sealed class Logger
    {
        private readonly string _name;

        internal static Logger Create<T>()
        {
            return new Logger(typeof(T).GetPrettyName());
        }

        internal static Logger Create(string logger)
        {
            return new Logger(logger);
        }

        private Logger(string name)
        {
            _name = name;
        }

        [StringFormatMethod("template")]
        public delegate void Log(string loggerName, LogLevel logLevel, string template, object[] args, Exception ex = null);

        public static event Log OnLog;
        public static IDictionary<string, object> GetLogicalThreadContexts() => LogicalThreadContext.GetAll();

        [StringFormatMethod("template")]
        internal void Fatal(string template, params object[] args)
        {
            Fatal(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Fatal(Exception ex, string template, params object[] args)
        {
            OnLog?.Invoke(_name, LogLevel.Fatal, template, args, ex);
        }

        [StringFormatMethod("template")]
        internal void Error(string template, params object[] args)
        {
            Error(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Error(Exception ex, string template, params object[] args)
        {
            OnLog?.Invoke(_name, LogLevel.Error, template, args, ex);
        }

        [StringFormatMethod("template")]
        internal void Warning(string template, params object[] args)
        {
            Warning(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Warning(Exception ex, string template, params object[] args)
        {
            OnLog?.Invoke(_name, LogLevel.Warning, template, args, ex);
        }

        [StringFormatMethod("template")]
        internal void Info(string template, params object[] args)
        {
            Info(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Info(Exception ex, string template, params object[] args)
        {
            OnLog?.Invoke(_name, LogLevel.Info, template, args, ex);
        }

        [StringFormatMethod("template")]
        internal void Debug(string template, params object[] args)
        {
            Debug(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Debug(Exception ex, string template, params object[] args)
        {
            OnLog?.Invoke(_name, LogLevel.Debug, template, args, ex);
        }

        [StringFormatMethod("template")]
        internal void Trace(string template, params object[] args)
        {
            Trace(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Trace(Exception ex, string template, params object[] args)
        {
            OnLog?.Invoke(_name, LogLevel.Trace, template, args, ex);
        }

        private static readonly LogicalThreadContext Context = new LogicalThreadContext();
        internal LogicalThreadContext LogicalThreadContext => Context;
    }
}