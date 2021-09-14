using System;
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
        private readonly INetworkServer _networkServer;
        private readonly IProtocolResolver _protocolResolver;
        private readonly IConfiguration _configuration;
        private readonly Action<AmqpConnectionSession> _subscription;
        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private readonly List<Task> _tasks = new List<Task>();

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
            var networkClientReceiver = new List<IAsyncDisposable>();
            var connectionSession = new List<IDisposable>();

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
                                networkClientReceiver.Add(client.StartReceiving());
                                connectionSession.Add(session);
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
                try
                {
                    await networkClientReceiver.DisposeAllAsync()
                        .ConfigureAwait(false);
                }
                finally
                {
                    await Task.WhenAll(connectionSession.Select(disposable =>
                    {
                        disposable.Dispose();
                        return Task.CompletedTask;
                    }));
                    cts.Dispose();
                }
            });
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