using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.It.With.Amqp.Protocol.Extensions
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string str, char delimiter)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var sections = str
                .ToLower()
                .Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(section => section.First().ToString().ToUpper() + string.Join(string.Empty, section.Skip(1)));

            return string.Concat(sections);
        }

        public static string ToCamelCase(this string str, char delimiter)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var result = str.ToPascalCase(delimiter);
            return result.First().ToString().ToLower() + string.Join(string.Empty, result.Skip(1));
        }

        public static string[] SplitOnUpperCase(this string str)
        {
            var strings = new List<string>();

            if (str == null)
            {
                return strings.ToArray();
            }

            var splitString = "";
            foreach (var chr in str)
            {
                if (splitString != "" && char.IsUpper(chr))
                {
                    strings.Add(splitString);
                    splitString = "";
                }

                splitString += chr;
            }

            if (splitString != "")
            {
                strings.Add(splitString);
            }

            return strings.ToArray();
        }
    }
}