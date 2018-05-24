using System.Collections.Generic;
using System.Linq;

namespace Test.It.With.Amqp.Extensions
{
    internal static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Any() == false;
        }
    }
}