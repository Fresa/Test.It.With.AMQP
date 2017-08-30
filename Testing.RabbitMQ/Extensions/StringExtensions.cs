using System.Linq;

namespace Test.It.With.RabbitMQ.Extensions
{
    public static class StringExtensions
    {
        public static string SeperateOnCase(this string s, char separator, bool separateOnLowerCase = false)
        {
            return string.Concat(s.Select((x, i) =>
                i > 0 && (separateOnLowerCase ? char.IsLower(x) : char.IsUpper(x)) ? separator + x.ToString() : x.ToString()));
        }
    }
}