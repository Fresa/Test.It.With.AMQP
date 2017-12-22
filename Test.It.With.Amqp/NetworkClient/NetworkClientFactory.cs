using System;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class NetworkClientFactory : INetworkClientFactory
    {
        private readonly ProtocolVersion _protocolVersion;
        private Action<AmqpTestServer> _subscription;

        public NetworkClientFactory(ProtocolVersion protocolVersion)
        {
            _protocolVersion = protocolVersion;
        }

        public void OnNetworkClientCreated(Action<AmqpTestServer> subscription)
        {
            _subscription = subscription;
        }

        public INetworkClient Create()
        {
            var framework  = new AmqpTestServer(_protocolVersion);
            _subscription(framework);
            return framework.Client;
        }
    }
}