using System.Collections.Generic;
using System.Linq;

namespace Test.It.With.Amqp.ProtocolGenerator
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> list)
        {
            return list.Where(item => item != null);
        }
    }
}