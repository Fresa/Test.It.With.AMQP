using System;
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

        public static string ToPascalCase(this string str, char delimiter)
        {
            var sections = str
                .ToLower()
                .Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(section => section.First().ToString().ToUpper() + string.Join(string.Empty, section.Skip(1)));

            return string.Concat(sections);
        }

        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.First().ToString().ToLower() + string.Concat(str.Skip(1));
        }
    }
}