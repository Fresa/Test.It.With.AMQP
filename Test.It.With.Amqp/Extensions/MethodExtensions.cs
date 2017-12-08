using Newtonsoft.Json;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Extensions
{
    internal static class MethodExtensions
    {
        public static string Serialize(this IMethod method)
        {
            return JsonConvert.SerializeObject(method);
        }
    }
}