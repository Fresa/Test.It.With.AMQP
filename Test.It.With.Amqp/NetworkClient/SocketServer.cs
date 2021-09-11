using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Log.It;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class SocketServer : INetworkServer, IDisposable
    {
        private readonly ConcurrentQueue<INetworkClient> _clients = new ConcurrentQueue<INetworkClient>();
        private readonly ConcurrentQueue<INetworkClient> _waitingClients = new ConcurrentQueue<INetworkClient>();
        private readonly SemaphoreSlim _clientAvailable = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private Task _acceptingClientsBackgroundTask;
        private Socket _clientAcceptingSocket;
        private static readonly ILogger Logger = LogFactory.Create<SocketServer>();

        internal int Port { get; private set; }
        internal IPAddress Address { get; private set; } = IPAddress.Any;

        public async Task<INetworkClient> WaitForConnectedClientAsync(CancellationToken cancellationToken = default)
        {
            await _clientAvailable
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            _waitingClients.TryDequeue(out var client);
            Logger.Debug("Client accepted {@client}", client);
            _clients.Enqueue(client);
            return client;
        }

        internal static SocketServer Start()
        {
            return Start(IPAddress.Any);
        }

        internal static SocketServer Start(
            IPAddress address,
            int port = 0)
        {
            var server = new SocketServer();
            server.Connect(address, port);
            server.StartAcceptingClients();
            return server;
        }

        private void StartAcceptingClients()
        {
            _acceptingClientsBackgroundTask = Task.Run(async () =>
            {
                while (_cancellationSource.IsCancellationRequested == false)
                    try
                    {
                        var clientSocket = await _clientAcceptingSocket
                            .AcceptAsync()
                            .ConfigureAwait(false);
                        Logger.Debug("Client connected {@clientSocket}", clientSocket);

                        _waitingClients.Enqueue(
                            new SocketNetworkClient(clientSocket));
                        _clientAvailable.Release();
                    }
                    catch when (_cancellationSource.IsCancellationRequested)
                    {
                        // Shutdown in progress
                        return;
                    }
            });
        }

        private void Connect(IPAddress address, int port)
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
            Logger.Info("Listening on {@endpoint}", localEndPoint);
        }

        public void Dispose()
        {
            _cancellationSource.Cancel();
            try
            {
                _clientAcceptingSocket.Shutdown(SocketShutdown.Both);
                _clientAcceptingSocket.Close();
            }
            catch
            {
            } // Ignore unhandled exceptions during shutdown 
            finally
            {
                _clientAcceptingSocket.Dispose();
            }

            _acceptingClientsBackgroundTask.GetAwaiter().GetResult();
            while (_clients.TryDequeue(out var client))
            {
                client.Dispose();
            }
        }
    }
}