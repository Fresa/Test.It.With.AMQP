using System;
using System.Net;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp
{
    public sealed class SocketAmqpTestFramework : AmqpTestFramework
    {
        private readonly SocketNetworkClientFactory _factory;

        internal SocketAmqpTestFramework(IProtocolResolver protocolResolver, INetworkConfiguration configuration) 
        {
            var server = SocketServer.Start(configuration.IpAddress, configuration.Port);
            Port = server.Port;
            Address = Equals(configuration.IpAddress, IPAddress.Any) ? IPAddress.Loopback : configuration.IpAddress;
            _factory = new SocketNetworkClientFactory(server, protocolResolver, configuration, AddSession);
            AsyncDisposables.Add(_factory);
            Disposables.Add(server);
        }

        internal SocketAmqpTestFramework(IProtocolResolver protocolResolver) 
            : this(protocolResolver, new DefaultNetworkConfiguration())
        {
            
        }

        public int Port { get; set; }

        public IPAddress Address { get; }

        public IAsyncDisposable Start()
        {
            return _factory.StartReceivingClients();
        }
    }
}