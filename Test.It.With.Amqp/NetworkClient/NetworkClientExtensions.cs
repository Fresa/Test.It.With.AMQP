using System.IO;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.NetworkClient
{
    public static class NetworkClientExtensions
    {
        public static void Send(this INetworkClient networkClient, Frame frame)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new AmqpWriter(stream))
                {
                    frame.WriteTo(writer);
                }
                var bytes = stream.ToArray();
                networkClient.Send(bytes, 0, bytes.Length);
            }
        }
    }
}