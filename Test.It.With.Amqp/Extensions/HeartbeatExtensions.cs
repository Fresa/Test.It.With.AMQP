using Newtonsoft.Json;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Extensions
{
    internal static class HeartbeatExtensions
    {
        public static string Serialize(this IHeartbeat heartbeat)
        {
            return JsonConvert.SerializeObject(heartbeat);
        }
    }
}