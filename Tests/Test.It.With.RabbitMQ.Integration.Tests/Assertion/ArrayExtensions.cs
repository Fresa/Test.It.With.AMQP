using System.Text;
using Newtonsoft.Json;

namespace Test.It.With.RabbitMQ.Integration.Tests.Assertion
{
    internal static class ArrayExtensions
    {
        public static T Deserialize<T>(this byte[] array)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(array));
        }
    }
}