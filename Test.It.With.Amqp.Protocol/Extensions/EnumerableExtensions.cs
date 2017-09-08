using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.It.With.Amqp.Protocol.Extensions
{
    public static class EnumerableExtensions
    {
        public static IDictionary<int, T> WithIndex<T>(this IEnumerable<T> list)
        {
            return list
                .Select((item, i) => 
                    new {Index = i, Item = item})
                .ToDictionary(
                    arg => arg.Index, 
                    arg => arg.Item);
        }

        public static string Join<T>(this IEnumerable<T> enumerable, string delimiter, string lastDelimiter, Func<T, string> converter)
        {
            var list = enumerable.ToList();
            if (list.Any() == false)
            {
                return string.Empty;
            }

            if (list.Count == 1)
            {
                return converter(list.First());
            }

            return string.Join(delimiter, list.Take(list.Count - 1).Select(converter)) + lastDelimiter + converter(list.Last());
        }
    }
}