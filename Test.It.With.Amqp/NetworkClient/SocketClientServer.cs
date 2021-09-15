using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Test.It.With.Amqp.System;

namespace Test.It.With.Amqp.NetworkClient
{
    internal sealed class SocketClientServer : IServer
    {
        private readonly SocketServer _socketServer;
        private readonly SocketNetworkClientFactory _socketNetworkClientFactory;
        private readonly IAsyncDisposable _clientReceiver;
        private readonly INetworkClientServer _networkClientServer;

        private SocketClientServer(SocketServer server, SocketNetworkClientFactory socketNetworkClientFactory)
        {
            _socketServer = server;
            _socketNetworkClientFactory = socketNetworkClientFactory;

            Port = server.Port;
            Address = server.Address.Equals(IPAddress.Any) ? IPAddress.Loopback :
                server.Address.Equals(IPAddress.IPv6Any) ? IPAddress.IPv6Loopback :
                server.Address;

            _networkClientServer = server.StartAcceptingClients();
            _clientReceiver = socketNetworkClientFactory.StartReceivingClients(_networkClientServer);
        }

        internal static IServer StartForwardingClients(SocketServer server, SocketNetworkClientFactory socketNetworkClientFactory)
        {
            return new SocketClientServer(server, socketNetworkClientFactory);
        }

        public ValueTask DisposeAsync() =>
            new ValueTask(
                new List<IAsyncDisposable>
                {
                    _clientReceiver,
                    _networkClientServer,
                    _socketNetworkClientFactory,
                    _socketServer,
                }.DisposeAllAsync());

        public int Port { get; }
        public IPAddress Address { get; }
    }
}