using System;

namespace Logging.Loggers.NLog
{
    public class NLogMappedDiagnosticsLogicalContext : ILogContext
    {
        public void Set(string key, string value)
        {
            NLogLogicalThreadContext.Set(key, value);
        }

        public string Get(string key)
        {
            return NLogLogicalThreadContext.Get(key);
        }

        public string Get(string key, IFormatProvider formatProvider)
        {
            return NLogLogicalThreadContext.Get(key, formatProvider);
        }

        public void Remove(string key)
        {
            NLogLogicalThreadContext.Remove(key);
        }
    }
}