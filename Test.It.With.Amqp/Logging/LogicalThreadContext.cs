using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test.It.With.Amqp.Logging
{
    internal sealed class LogicalThreadContext
    {
        private static readonly ConcurrentDictionary<string, AsyncLocal<object>> CallContext =
            new ConcurrentDictionary<string, AsyncLocal<object>>();

        private static T GetCallContextValue<T>(string key)
        {
            if (CallContext.TryGetValue(key, out var value) == false) return default;

            return (T) value.Value;
        }

        private static void SetCallContextValue(string key, object value)
        {
            CallContext.GetOrAdd(key, _ => new AsyncLocal<object>()).Value = value;
        }

        private static void RemoveCallContextValue(string key)
        {
            if (CallContext.TryGetValue(key, out var asyncLocal))
                asyncLocal.Value = null;
        }

        /// <summary>
        /// Gets a value from the logical thread context
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="formatProvider">Provides a formatter that is used to format the value</param>
        /// <returns>Formatted value</returns>
        public string Get(string key, IFormatProvider formatProvider)
        {
            return Convert.ToString(GetCallContextValue<object>(key), formatProvider);
        }

        /// <summary>
        /// Gets a value from the logical thread context
        /// </summary>
        /// <typeparam name="T">Any type that is serializable</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public T Get<T>(string key)
        {
            return GetCallContextValue<T>(key);
        }

        /// <summary>
        /// Gets all values from the logical thread context
        /// </summary>
        /// <returns>All context key/values</returns>
        public static IDictionary<string, object> GetAll()
        {
            return CallContext.ToDictionary(context => context.Key, context => context.Value.Value);
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

        /// <summary>
        /// Check if a value is present in the logical thread context
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if value is present otherwise false</returns>
        public bool Contains(string key)
        {
            return CallContext.ContainsKey(key);
        }
    }
}