using System;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class NetworkClientFactory : INetworkClientFactory
    {
        private readonly IProtocolResolver _protocolResolver;
        private readonly IConfiguration _configuration;
        private Action<AmqpConnectionSession> _subscription;

        public NetworkClientFactory(IProtocolResolver protocolResolver, IConfiguration configuration)
        {
            _protocolResolver = protocolResolver;
            _configuration = configuration;
        }

        public void OnNetworkClientCreated(Action<AmqpConnectionSession> subscription)
        {
            _subscription = subscription;
        }

        public INetworkClient Create()
        {
            var framework  = new AmqpConnectionSession(_protocolResolver, _configuration);
            _subscription(framework);
            return framework.Client;
        }
    }
}