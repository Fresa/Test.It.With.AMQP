using System;
using Test.It.With.Amqp.Logging;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class InMemoryNetworkClientFactory : INetworkClientFactory
    {
        private readonly IProtocolResolver _protocolResolver;
        private readonly IConfiguration _configuration;
        private Func<AmqpConnectionSession, IDisposable> _subscription;
        private readonly InternalRoutedNetworkClientFactory _networkClientFactory = new InternalRoutedNetworkClientFactory();
        private readonly Logger _logger = Logger.Create<InMemoryNetworkClientFactory>();

        public InMemoryNetworkClientFactory(IProtocolResolver protocolResolver, IConfiguration configuration)
        {
            _protocolResolver = protocolResolver;
            _configuration = configuration;
            _networkClientFactory.OnException += exception => 
                _logger.Error(exception, "Test framework error.");
        }

        public void OnNetworkClientCreated(Func<AmqpConnectionSession, IDisposable> subscription)
        {
            _subscription = subscription;
        }

        public INetworkClient Create()
        {
            var client = _networkClientFactory.Create(out var serverNetworkClient);
            var session = new AmqpConnectionSession(_protocolResolver, _configuration, serverNetworkClient);
            var unsubscribe = _subscription(session);
            client.Disconnected += (sender, args) => unsubscribe.Dispose();
            return client;
        }
    }
}