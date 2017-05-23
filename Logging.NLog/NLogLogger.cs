using System;
using NLog;

namespace Log.It.With.NLog
{
    public class NLogLogger : ILogger
    {
        private readonly Logger _logger;
        private readonly ILogContext _logContext;

        public NLogLogger(string type, ILogContext logContext)
        {
            _logger = LogManager.GetLogger(type);
            _logContext = logContext;
        }


        public void Fatal(Exception ex, IFormatProvider formatProvider, string template, params object[] args)
        {
            _logger.Fatal(ex, formatProvider, template, args);
        }

        public void Fatal(Exception ex, string template, params object[] args)
        {
            _logger.Fatal(ex, template, args);
        }

        public void Fatal<T>(IFormatProvider formatProvider, T msg)
        {
            _logger.Fatal(formatProvider, msg);
        }

        public void Fatal(IFormatProvider formatProvider, object msg)
        {
            _logger.Fatal(formatProvider, msg);
        }

        public void Fatal(Exception ex, string msg)
        {
            _logger.Fatal(ex, msg);
        }

        public void Fatal(string template, params object[] args)
        {
            _logger.Fatal(template, args);
        }

        
        public void Error(Exception ex, IFormatProvider formatProvider, string template, params object[] args)
        {
            _logger.Error(ex, formatProvider, template, args);
        }
        
        public void Error(Exception ex, string template, params object[] args)
        {
            _logger.Error(ex, template, args);
        }

        public void Error<TMessage>(IFormatProvider formatProvider, TMessage msg)
        {
            _logger.Error(formatProvider, msg);
        }

        public void Error(IFormatProvider formatProvider, object msg)
        {
            _logger.Error(formatProvider, msg);
        }

        public void Error(Exception ex, string msg)
        {
            _logger.Fatal(ex, msg);
        }

        public void Error(string template, params object[] args)
        {
            _logger.Error(template, args);
        }


        public void Warning(Exception ex, IFormatProvider formatProvider, string template, params object[] args)
        {
            _logger.Warn(ex, formatProvider, template, args);
        }
        
        public void Warning(Exception ex, string template, params object[] args)
        {
            _logger.Warn(ex, template, args);
        }

        public void Warning<TMessage>(IFormatProvider formatProvider, TMessage msg)
        {
            _logger.Warn(formatProvider, msg);
        }

        public void Warning(IFormatProvider formatProvider, object msg)
        {
            _logger.Warn(formatProvider, msg);
        }

        public void Warning(Exception ex, string msg)
        {
            _logger.Warn(ex, msg);
        }

        public void Warning(string template, params object[] args)
        {
            _logger.Warn(template, args);
        }


        public void Info(Exception ex, IFormatProvider formatProvider, string template, params object[] args)
        {
            _logger.Info(ex, formatProvider, template, args);
        }

        public void Info(Exception ex, string template, params object[] args)
        {
            _logger.Info(ex, template, args);
        }

        public void Info<TMessage>(IFormatProvider formatProvider, TMessage msg)
        {
            _logger.Info(formatProvider, msg);
        }

        public void Info(IFormatProvider formatProvider, object msg)
        {
            _logger.Info(formatProvider, msg);
        }

        public void Info(Exception ex, string msg)
        {
            _logger.Info(ex, msg);
        }

        public void Info(string template, params object[] args)
        {
            _logger.Info(template, args);
        }


        public void Debug(Exception ex, IFormatProvider formatProvider, string template, params object[] args)
        {
            _logger.Debug(ex, formatProvider, template, args);
        }

        public void Debug(Exception ex, string template, params object[] args)
        {
            _logger.Debug(ex, template, args);
        }

        public void Debug<TMessage>(IFormatProvider formatProvider, TMessage msg)
        {
            _logger.Debug(formatProvider, msg);
        }

        public void Debug(IFormatProvider formatProvider, object msg)
        {
            _logger.Debug(formatProvider, msg);
        }

        public void Debug(Exception ex, string msg)
        {
            _logger.Debug(ex, msg);
        }
        
        public void Debug(string template, params object[] args)
        {
            _logger.Debug(template, args);
        }


        public void Trace(Exception ex, IFormatProvider formatProvider, string template, params object[] args)
        {
            _logger.Trace(ex, formatProvider, template, args);
        }

        public void Trace(Exception ex, string template, params object[] args)
        {
            _logger.Trace(ex, template, args);
        }

        public void Trace<TMessage>(IFormatProvider formatProvider, TMessage msg)
        {
            _logger.Trace(formatProvider, msg);
        }

        public void Trace(IFormatProvider formatProvider, object msg)
        {
            _logger.Trace(formatProvider, msg);
        }

        public void Trace(Exception ex, string msg)
        {
            _logger.Trace(ex, msg);
        }
        
        public void Trace(string template, params object[] args)
        {
            _logger.Trace(template, args);
        }


        public ILogContext LogicalThread => _logContext;
    }
}