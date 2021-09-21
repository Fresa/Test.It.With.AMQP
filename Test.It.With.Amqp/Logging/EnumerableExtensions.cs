using System.Collections.Generic;

namespace Test.It.With.Amqp.Logging
{
    internal static class EnumerableExtensions
    {
        internal static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }
    }
}