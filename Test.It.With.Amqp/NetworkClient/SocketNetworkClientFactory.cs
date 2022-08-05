using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.System;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class SocketNetworkClientFactory : IAsyncDisposable
    {
        private readonly IProtocolResolver _protocolResolver;
        private readonly IConfiguration _configuration;
        private readonly Func<AmqpConnectionSession, IDisposable> _subscribe;
        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private readonly List<Task> _tasks = new List<Task>();

        public SocketNetworkClientFactory(
            IProtocolResolver protocolResolver,
            IConfiguration configuration,
            Func<AmqpConnectionSession, IDisposable> subscribe)
        {
            _protocolResolver = protocolResolver;
            _configuration = configuration;
            _subscribe = subscribe;
        }

        public ClientSessions StartReceivingClients(INetworkClientServer networkClientServer)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            var token = cts.Token;

            var activeSessions = new ConcurrentDictionary<ConnectionId, DisconnectSessionAsync>();
            var disconnectedSessions = new ConcurrentQueue<ConnectionId>();
            var disconnectedSessionSignaler = new SemaphoreSlim(0);

            // ReSharper disable once MethodSupportsCancellation
            // Will be handled by the disposable returned
            var clientReceivingTask = Task.Run(
                    async () =>
                    {
                        while (cts.IsCancellationRequested == false)
                        {
                            try
                            {
                                var client = await networkClientServer
                                    .WaitForConnectedClientAsync(token)
                                    .ConfigureAwait(false);
                                var session = new AmqpConnectionSession(_protocolResolver, _configuration, client);
                                var unsubscribe = _subscribe(session);
                                var signalDisconnect = new SemaphoreSlim(0);
                                client.Disconnected += SignalDisconnectOnStart;
                                var receiver = client.StartReceiving();

                                activeSessions.TryAdd(session.ConnectionId, Disconnect);
                                client.Disconnected += OnClientDisconnected;
                                // Disconnection happened before we could start accept disconnections
                                if (signalDisconnect.CurrentCount > 0)
                                {
                                    OnClientDisconnected(this, EventArgs.Empty);
                                }
                                client.Disconnected -= SignalDisconnectOnStart;

                                void SignalDisconnectOnStart(object sender, EventArgs args)
                                {
                                    signalDisconnect.Release();
                                }

                                async ValueTask Disconnect(CancellationToken _)
                                {
                                    // Dispose in reverse dependency order
                                    await receiver.DisposeAsync()
                                        .ConfigureAwait(false);
                                    unsubscribe.Dispose();
                                    session.Dispose();
                                    client.Dispose();
                                }

                                void OnClientDisconnected(object sender, EventArgs args)
                                {
                                    disconnectedSessions.Enqueue(session.ConnectionId);
                                    disconnectedSessionSignaler.Release();
                                }
                            }
                            catch when (cts.IsCancellationRequested)
                            {
                                return;
                            }
                        }
                    });
            _tasks.Add(clientReceivingTask);

            // ReSharper disable once MethodSupportsCancellation
            // Will be handled by the disposable returned
            var clientsDisconnectingTask = Task.Run(
                async () =>
                {
                    while (cts.IsCancellationRequested == false)
                    {
                        try
                        {
                            await disconnectedSessionSignaler.WaitAsync(token)
                                .ConfigureAwait(false);
                            if (!disconnectedSessions.TryDequeue(out var disconnectedSessionId))
                            {
                                throw new InvalidOperationException(
                                    "Got signal about disconnect session but no disconnected session found");
                            }

                            if (activeSessions.TryRemove(disconnectedSessionId, out var disconnectSessionAsync))
                            {
                                await disconnectSessionAsync(token)
                                    .ConfigureAwait(false);
                            }
                        }
                        catch when (cts.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
            );
            _tasks.Add(clientsDisconnectingTask);
            
            return new ClientSessions(activeSessions, new AsyncDisposableAction(async () =>
            {
                cts.Cancel();
                await Task.WhenAll(clientReceivingTask, clientsDisconnectingTask)
                    .ConfigureAwait(false);

                await activeSessions.Keys.Select(id =>
                        activeSessions.TryRemove(id, out var disconnectSessionAsync)
                            ? disconnectSessionAsync(_cancellationTokenSource.Token)
                            : new ValueTask())
                    .WhenAllAsync()
                    .ConfigureAwait(false);
                
                cts.Dispose();
            }));
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            try
            {
                await Task.WhenAll(_tasks)
                    .ConfigureAwait(false);
            }
            catch 
            {
            }
            _cancellationTokenSource.Dispose();
        }
    }
}
