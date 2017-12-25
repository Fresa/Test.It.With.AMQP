using System.IO;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.Amqp.NetworkClient
{
    public static class NetworkClientExtensions
    {
        public static void Send(this INetworkClient networkClient, IFrame frame)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Amqp091Writer(stream))
                {
                    frame.WriteTo(writer);
                }
                var bytes = stream.ToArray();
                networkClient.Send(bytes, 0, bytes.Length);
            }
        }
    }
}