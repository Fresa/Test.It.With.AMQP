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

            var sessions = new ConcurrentDictionary<ConnectionId, Func<CancellationToken, ValueTask>>();
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

                                client.Disconnected += OnClientOnDisconnected;
                                var receiver = client.StartReceiving();
                                
                                sessions.TryAdd(session.ConnectionId, _ =>
                                {
                                    unsubscribe.Dispose();
                                    session.Dispose();
                                    client.Dispose();
                                    return receiver.DisposeAsync();
                                });

                                void OnClientOnDisconnected(object sender, EventArgs args)
                                {
                                    client.Disconnected -= OnClientOnDisconnected;
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
                            if (!disconnectedSessions.TryDequeue(out var disconnectedSession))
                            {
                                throw new InvalidOperationException(
                                    "Got signal about disconnect session but no disconnected session found");
                            }

                            Func<CancellationToken, ValueTask> disconnectAsync;
                            while (!sessions.TryRemove(disconnectedSession, out disconnectAsync))
                            {
                                // Race condition: a client disconnected fast and no session subscriptions have been registered yet
                                Thread.SpinWait(1);
                            }

                            await disconnectAsync(default)
                                .ConfigureAwait(false);
                        }
                        catch when (cts.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
            );
            _tasks.Add(clientsDisconnectingTask);

            return new ClientSessions(sessions, new AsyncDisposableAction(async () =>
            {
                cts.Cancel();
                await Task.WhenAll(clientReceivingTask, clientsDisconnectingTask)
                    .ConfigureAwait(false);

                await sessions.Values.Select(disconnect => disconnect(default))
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