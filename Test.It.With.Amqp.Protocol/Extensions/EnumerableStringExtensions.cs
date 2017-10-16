using System.Collections.Generic;

namespace Test.It.With.Amqp.Protocol.Extensions
{
    public static class EnumerableStringExtensions
    {
        public static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }
    }
}