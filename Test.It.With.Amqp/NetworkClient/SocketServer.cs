using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Test.It.With.Amqp.Logging;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class SocketServer : IAsyncDisposable
    {
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private Socket _clientAcceptingSocket;
        private static readonly Logger Logger = Logger.Create<SocketServer>();

        internal int Port { get; private set; }
        internal IPAddress Address { get; private set; } = IPAddress.Any;

        internal static SocketServer Connect(
            IPAddress address,
            int port = 0)
        {
            var server = new SocketServer();
            server.ConnectAcceptingSocket(address, port);
            return server;
        }

        internal INetworkClientServer StartAcceptingClients() => 
            NetworkClientServer.StartAcceptingClients(_clientAcceptingSocket, _cancellationSource.Token);

        private void ConnectAcceptingSocket(IPAddress address, int port)
        {
            var endPoint = new IPEndPoint(address, port);

            _clientAcceptingSocket = new Socket(
                address.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            _clientAcceptingSocket.Bind(endPoint);
            var localEndPoint = (IPEndPoint)_clientAcceptingSocket.LocalEndPoint;
            Port = localEndPoint.Port;
            Address = localEndPoint.Address;
            _clientAcceptingSocket.Listen(100);
            Logger.Info("Listening on {@endpoint}", new
            {
                IPAddress = localEndPoint.Address.ToString(),
                localEndPoint.Port,
                localEndPoint.AddressFamily,
                _clientAcceptingSocket.ProtocolType
            });
        }

        public ValueTask DisposeAsync()
        {
            _cancellationSource.Cancel();
            try
            {
                _clientAcceptingSocket.Close();
            }
            catch
            {
            }

            try
            {
                _clientAcceptingSocket.Dispose();
            }
            catch
            {
            }
            return new ValueTask();
        }
    }
}