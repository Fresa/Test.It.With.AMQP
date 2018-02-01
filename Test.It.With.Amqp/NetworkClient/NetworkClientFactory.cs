using System;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class NetworkClientFactory : INetworkClientFactory
    {
        private readonly ProtocolVersion _protocolVersion;
        private Action<AmqpConnectionSession> _subscription;

        public NetworkClientFactory(ProtocolVersion protocolVersion)
        {
            _protocolVersion = protocolVersion;
        }

        public void OnNetworkClientCreated(Action<AmqpConnectionSession> subscription)
        {
            _subscription = subscription;
        }

        public INetworkClient Create()
        {
            var framework  = new AmqpConnectionSession(_protocolVersion);
            _subscription(framework);
            return framework.Client;
        }
    }
}