using System;
using System.Collections.Generic;
using Test.It.With.Amqp.Logging.Annotations;

namespace Test.It.With.Amqp.Logging
{
    public abstract class Logger
    {
        [StringFormatMethod("template")] 
        public abstract void Fatal(string loggerName, string template, object[] args, Exception ex = null);

        [StringFormatMethod("template")] 
        public abstract void Error(string loggerName, string template, object[] args, Exception ex = null);

        [StringFormatMethod("template")] 
        public abstract void Warning(string loggerName, string template, object[] args, Exception ex = null);

        [StringFormatMethod("template")] 
        public abstract void Info(string loggerName, string template, object[] args, Exception ex = null);

        [StringFormatMethod("template")] 
        public abstract void Debug(string loggerName, string template, object[] args, Exception ex = null);

        [StringFormatMethod("template")] 
        public abstract void Trace(string loggerName, string template, object[] args, Exception ex = null);
        
        protected IDictionary<string, object> GetLogicalThreadContexts() => LogicalThreadContext.GetAll();
    }
}