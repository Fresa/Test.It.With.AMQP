using System.Net;

namespace Test.It.With.Amqp
{
    internal class DefaultNetworkConfiguration : DefaultConfiguration, INetworkConfiguration
    {
        public DefaultNetworkConfiguration(IPAddress ipAddress, int port = 0)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public DefaultNetworkConfiguration() : this(IPAddress.IPv6Loopback)
        {

        }

        public IPAddress IpAddress { get; }
        public int Port { get; }
    }
}