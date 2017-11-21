using System;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class InternalRoutedNetworkClientFactory : INetworkClientFactory, IDisposable
    {
        private readonly InternalRoutedNetworkClient _serverNetworkClient;

        public InternalRoutedNetworkClientFactory(out INetworkClient serverNetworkClient)
        {
            // todo: should generate a new server client on each network creation
            serverNetworkClient = _serverNetworkClient = new InternalRoutedNetworkClient();
        }

        public INetworkClient Create()
        {
            var clientNetworkClient = new InternalRoutedNetworkClient();

            void OnServerDisconnect(object sender, EventArgs args)
            {
                clientNetworkClient.SendReceived -= _serverNetworkClient.TriggerReceive;
                clientNetworkClient.Dispose();
            }

            _serverNetworkClient.SendReceived += clientNetworkClient.TriggerReceive;
            _serverNetworkClient.Disconnected += OnServerDisconnect;

            void OnClientDisconnected(object sender, EventArgs args)
            {
                _serverNetworkClient.SendReceived -= clientNetworkClient.TriggerReceive;
                _serverNetworkClient.Disconnected -= OnServerDisconnect;
            }

            clientNetworkClient.SendReceived += (sender, args) =>
            {
                try
                {
                    _serverNetworkClient.TriggerReceive(sender, args);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(ex);
                    // todo: need to close properly (send Close)
                    OnServerDisconnect(_serverNetworkClient, EventArgs.Empty);
                }
            };
            clientNetworkClient.Disconnected += OnClientDisconnected;

            return clientNetworkClient;
        }

        public event Action<Exception> OnException;

        public void Dispose()
        {
            _serverNetworkClient.Dispose();
        }
    }
}