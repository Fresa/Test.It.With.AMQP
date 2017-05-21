using System;
using System.Globalization;
using System.Runtime.Remoting.Messaging;

namespace Logging.Loggers.NLog
{
    public static class NLogLogicalThreadContext
    {
        private const string KeyPrefix = "NLog.LogicalThreadContext";

        private static string GetCallContextKey(string key)
        {
            return $"{KeyPrefix}.{key}";
        }

        private static object GetCallContextValue(string key)
        {
            return CallContext.LogicalGetData(GetCallContextKey(key));
        }

        private static void SetCallContextValue(string key, object value)
        {
            if (value.IsSerializable())
            {
                CallContext.LogicalSetData(GetCallContextKey(key), value);
            }
            throw new NotSupportedException($"{key} with value of type {value.GetType().FullName} is not serializable.");
        }

        private static void RemoveCallContextValue(string key)
        {
            CallContext.FreeNamedDataSlot(GetCallContextKey(key));
        }

        public static string Get(string key)
        {
            return Get(key, CultureInfo.CurrentCulture);
        }

        public static string Get(string key, IFormatProvider formatProvider)
        {
            return Convert.ToString(GetCallContextValue(key), formatProvider);
        }

        public static T Get<T>(string key)
        {
            return (T)GetCallContextValue(key);
        }

        public static void Set<T>(string key, T value)
        {
            SetCallContextValue(key, value);
        }

        public static void Remove(string item)
        {
            RemoveCallContextValue(item);
        }
    }
}