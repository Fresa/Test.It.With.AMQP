using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp
{
    public sealed class InMemoryAmqpTestFramework : AmqpTestFramework
    {
        internal InMemoryAmqpTestFramework(IProtocolResolver protocolResolver, IConfiguration configuration)
        {
            var factory = new InMemoryNetworkClientFactory(protocolResolver, configuration);
            factory.OnNetworkClientCreated(AddSession);
            ConnectionFactory = factory;
        }

        internal InMemoryAmqpTestFramework(IProtocolResolver protocolResolver) : this(protocolResolver, new DefaultConfiguration())
        {
        }

        public INetworkClientFactory ConnectionFactory { get; }
    }
}