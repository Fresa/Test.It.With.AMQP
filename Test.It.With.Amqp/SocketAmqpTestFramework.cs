using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp
{
    public sealed class SocketAmqpTestFramework : AmqpTestFramework
    {
        private readonly IProtocolResolver _protocolResolver;
        private readonly INetworkConfiguration _configuration;

        internal SocketAmqpTestFramework(IProtocolResolver protocolResolver, INetworkConfiguration configuration)
        {
            _protocolResolver = protocolResolver;
            _configuration = configuration;
        }

        internal SocketAmqpTestFramework(IProtocolResolver protocolResolver)
            : this(protocolResolver, new DefaultNetworkConfiguration())
        {

        }

        public IServer Start()
        {
            var socketServer = SocketServer.Connect(_configuration.IpAddress, _configuration.Port);
            var socketNetworkClientFactory = new SocketNetworkClientFactory(_protocolResolver, _configuration, AddSession);

            return SocketClientServer.StartForwardingClients(socketServer, socketNetworkClientFactory);
        }
    }
}