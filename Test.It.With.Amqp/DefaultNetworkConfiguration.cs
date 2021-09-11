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

        public DefaultNetworkConfiguration() : this(IPAddress.Any)
        {

        }

        public IPAddress IpAddress { get; }
        public int Port { get; }
    }
}