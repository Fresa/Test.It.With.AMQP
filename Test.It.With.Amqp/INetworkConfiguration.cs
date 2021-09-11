using System.Net;

namespace Test.It.With.Amqp
{
    public interface INetworkConfiguration : IConfiguration
    {
        IPAddress IpAddress { get; }
        int Port { get; }
    }
}