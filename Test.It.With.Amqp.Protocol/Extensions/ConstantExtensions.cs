using System.Collections.Generic;
using System.Linq;
using Test.It.With.Amqp.Protocol.Definitions;

namespace Test.It.With.Amqp.Protocol.Extensions
{
    public static class ConstantExtensions
    {
        public static string GetExceptionName(this IDictionary<string, Constant> constants, int code)
        {
            return constants.Values
                .Where(pair => string.IsNullOrEmpty(pair.Class) == false)
                .Single(constant => constant.Value == code).Name.ToPascalCase('-');
        }
    }
}