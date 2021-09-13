using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.System;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class SocketNetworkClientFactory : IAsyncDisposable
    {
        private readonly INetworkServer _networkServer;
        private readonly IProtocolResolver _protocolResolver;
        private readonly IConfiguration _configuration;
        private Action<AmqpConnectionSession> _subscription;
        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();


        private List<Task> _tasks = new List<Task>();

        public SocketNetworkClientFactory(
            INetworkServer networkServer,
            IProtocolResolver protocolResolver,
            IConfiguration configuration,
            Action<AmqpConnectionSession> subscription)
        {
            _networkServer = networkServer;
            _protocolResolver = protocolResolver;
            _configuration = configuration;
            _subscription = subscription;
        }

        public IAsyncDisposable StartReceivingClients()
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            var token = cts.Token;
            var messageReceivers = new List<IAsyncDisposable>();

            // ReSharper disable once MethodSupportsCancellation
            // Will be handled by the disposable returned
            var clientReceivingTask = Task.Run(
                    async () =>
                    {
                        while (cts.IsCancellationRequested == false)
                        {
                            try
                            {
                                var client = await _networkServer
                                    .WaitForConnectedClientAsync(token)
                                    .ConfigureAwait(false);
                                var session = new AmqpConnectionSession(_protocolResolver, _configuration, client);
                                _subscription(session);
                                messageReceivers.Add(client.StartReceiving());
                            }
                            catch when (cts.IsCancellationRequested)
                            {
                                return;
                            }
                        }
                    });
            _tasks.Add(clientReceivingTask);

            return new AsyncDisposableAction(async () =>
            {
                cts.Cancel();
                await clientReceivingTask
                    .ConfigureAwait(false);
                await messageReceivers.DisposeAllAsync()
                    .ConfigureAwait(false);
                cts.Dispose();
            });
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            await Task.WhenAll(_tasks)
                .ConfigureAwait(false);
            _cancellationTokenSource.Dispose();
        }
    }
}