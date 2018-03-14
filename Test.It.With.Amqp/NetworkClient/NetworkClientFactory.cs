using System;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class NetworkClientFactory : INetworkClientFactory
    {
        private readonly IProtocolResolver _protocolResolver;
        private Action<AmqpConnectionSession> _subscription;

        public NetworkClientFactory(IProtocolResolver protocolResolver)
        {
            _protocolResolver = protocolResolver;
        }

        public void OnNetworkClientCreated(Action<AmqpConnectionSession> subscription)
        {
            _subscription = subscription;
        }

        public INetworkClient Create()
        {
            var framework  = new AmqpConnectionSession(_protocolResolver);
            _subscription(framework);
            return framework.Client;
        }
    }
}