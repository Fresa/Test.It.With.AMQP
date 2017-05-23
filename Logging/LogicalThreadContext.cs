using System;
using System.Runtime.Remoting.Messaging;

namespace Log.It
{
    public class LogicalThreadContext : ILogContext
    {
        private const string KeyPrefix = "Logging.LogicalThreadContext";

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
            CallContext.LogicalSetData(GetCallContextKey(key), value);
        }

        private static void RemoveCallContextValue(string key)
        {
            CallContext.FreeNamedDataSlot(GetCallContextKey(key));
        }

        /// <summary>
        /// Gets a value from the logical thread context
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="formatProvider">Provides a formatter that is used to format the value</param>
        /// <returns>Formatted value</returns>
        public string Get(string key, IFormatProvider formatProvider)
        {
            return Convert.ToString(GetCallContextValue(key), formatProvider);
        }

        /// <summary>
        /// Gets a value from the logical thread context
        /// </summary>
        /// <typeparam name="T">Any type that is serializable</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public T Get<T>(string key)
        {
            return (T)GetCallContextValue(key);
        }

        /// <summary>
        /// Sets a value in the logical thread context
        /// </summary>
        /// <typeparam name="T">Any type that is serializable</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Set<T>(string key, T value)
        {
            SetCallContextValue(key, value);
        }

        /// <summary>
        /// Removes a value from the logical thread context
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(string key)
        {
            RemoveCallContextValue(key);
        }
    }
}