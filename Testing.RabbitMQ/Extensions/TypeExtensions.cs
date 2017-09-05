using System;
using System.Linq;

namespace Test.It.With.RabbitMQ.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static string GetPrettyFullName(this Type type)
        {
            var prettyName = type.FullName;
            if (type.IsGenericType == false)
            {
                return prettyName;
            }

            if (prettyName.IndexOf('`') > 0)
            {
                prettyName = prettyName.Remove(prettyName.IndexOf('`'));
            }

            var genericArguments = type.GetGenericArguments()
                .Select(genericArgument => genericArgument.GetPrettyFullName());

            return $"{prettyName}<{string.Join(", ", genericArguments)}>";
        }

    }
}