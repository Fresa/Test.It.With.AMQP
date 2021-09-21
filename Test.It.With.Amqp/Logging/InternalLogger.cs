using System;
using Test.It.With.Amqp.Logging.Annotations;

namespace Test.It.With.Amqp.Logging
{
    internal sealed class InternalLogger
    {
        private readonly string _name;
        private readonly Action<LogMessage> _enqueueLogMessage;

        public InternalLogger(string name, Action<LogMessage> enqueueLogMessage)
        {
            _name = name;
            _enqueueLogMessage = enqueueLogMessage;
        }

        [StringFormatMethod("template")]
        internal void Fatal(string template, params object[] args)
        {
            Fatal(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Fatal(Exception ex, string template, params object[] args)
        {
            _enqueueLogMessage.Invoke(new LogMessage(_name, LogLevel.Fatal, template, args, ex));
        }

        [StringFormatMethod("template")]
        internal void Error(string template, params object[] args)
        {
            Error(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Error(Exception ex, string template, params object[] args)
        {
            _enqueueLogMessage.Invoke(new LogMessage(_name, LogLevel.Error, template, args, ex));
        }

        [StringFormatMethod("template")]
        internal void Warning(string template, params object[] args)
        {
            Warning(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Warning(Exception ex, string template, params object[] args)
        {
            _enqueueLogMessage.Invoke(new LogMessage(_name, LogLevel.Warning, template, args, ex));
        }

        [StringFormatMethod("template")]
        internal void Info(string template, params object[] args)
        {
            Info(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Info(Exception ex, string template, params object[] args)
        {
            _enqueueLogMessage.Invoke(new LogMessage(_name, LogLevel.Info, template, args, ex));
        }

        [StringFormatMethod("template")]
        internal void Debug(string template, params object[] args)
        {
            Debug(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Debug(Exception ex, string template, params object[] args)
        {
            _enqueueLogMessage.Invoke(new LogMessage(_name, LogLevel.Debug, template, args, ex));
        }

        [StringFormatMethod("template")]
        internal void Trace(string template, params object[] args)
        {
            Trace(null, template, args);
        }

        [StringFormatMethod("template")]
        internal void Trace(Exception ex, string template, params object[] args)
        {
            _enqueueLogMessage.Invoke(new LogMessage(_name, LogLevel.Trace, template, args, ex));
        }

        private static readonly LogicalThreadContext Context = new LogicalThreadContext();
        internal LogicalThreadContext LogicalThreadContext => Context;
    }
}