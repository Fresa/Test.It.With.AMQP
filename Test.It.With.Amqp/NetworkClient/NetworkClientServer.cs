using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Test.It.With.Amqp.Logging;

namespace Test.It.With.Amqp.NetworkClient
{
    internal sealed class NetworkClientServer : INetworkClientServer
    {
        private readonly Socket _clientAcceptingSocket;
        private readonly ConcurrentQueue<INetworkClient> _clients = new ConcurrentQueue<INetworkClient>();
        private readonly ConcurrentQueue<SocketNetworkClient> _waitingClients = new ConcurrentQueue<SocketNetworkClient>();
        private readonly SemaphoreSlim _clientAvailable = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private Task _acceptingClientsBackgroundTask = Task.CompletedTask;
        private static readonly Logger Logger = Logger.Create<NetworkClientServer>();

        private NetworkClientServer(Socket clientAcceptingSocket)
        {
            _clientAcceptingSocket = clientAcceptingSocket;
        }
            
        internal static INetworkClientServer StartAcceptingClients(Socket clientAcceptingSocket, CancellationToken cancellation)
        {
            var networkServer = new NetworkClientServer(clientAcceptingSocket);
            networkServer.StartAcceptingClients(cancellation);
            return networkServer;
        }
            
        private void StartAcceptingClients(CancellationToken cancellation)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationSource.Token, cancellation);
            // ReSharper disable once MethodSupportsCancellation
            // Handled by the disposable returned
            _acceptingClientsBackgroundTask = Task.Run((Func<Task>) (async () =>
            {
                while (cts.IsCancellationRequested == false)
                    try
                    {
                        var clientSocket = await _clientAcceptingSocket
                            .AcceptAsync()
                            .ConfigureAwait(false);

                        var networkClient = new SocketNetworkClient(clientSocket);
                        Logger.Debug("Client connected {@clientSocket}", networkClient.Serialize());

                        _waitingClients.Enqueue(networkClient);
                        _clientAvailable.Release();
                    }
                    catch when (cts.IsCancellationRequested)
                    {
                        // Shutdown in progress
                        return;
                    }
            }));
        }

        public async Task<IStartableNetworkClient> WaitForConnectedClientAsync(CancellationToken cancellationToken = default)
        {
            await _clientAvailable
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            _waitingClients.TryDequeue(out var client);
            Logger.Debug("Client accepted {@client}", client.Serialize());
            _clients.Enqueue(client);
            return client;
        }

        public async ValueTask DisposeAsync()
        {
            var exceptions = new List<Exception>();
            _cancellationSource.Cancel();
            try
            {
                _clientAcceptingSocket.Close();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                _clientAcceptingSocket.Dispose();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                await _acceptingClientsBackgroundTask
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            while (_clients.TryDequeue(out var client))
            {
                try
                {
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    ExceptionDispatchInfo.Capture(exceptions.First()).Throw();
                }

                throw new AggregateException(exceptions);
            }
        }
    }
}