using Newtonsoft.Json;

namespace Test.It.With.Amqp.Extensions
{
    internal static class MethodExtensions
    {
        public static string Serialize<T>(this T method) where T : class
        {
            return JsonConvert.SerializeObject(method);
        }
    }
}