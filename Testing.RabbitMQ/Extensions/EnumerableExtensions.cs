using System.Collections.Generic;
using System.Linq;

namespace Test.It.With.RabbitMQ.Extensions
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
    }
}