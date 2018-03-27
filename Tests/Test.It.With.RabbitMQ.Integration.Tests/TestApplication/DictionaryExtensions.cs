using System;
using System.Collections.Generic;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal static class DictionaryExtensions
    {
        internal static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> valueFactory)
        {
            if (dictionary.ContainsKey(key) == false)
            {
                dictionary.Add(key, valueFactory());
            }
            
            return dictionary[key];
        }
    }
}