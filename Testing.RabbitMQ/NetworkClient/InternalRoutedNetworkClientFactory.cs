using System;

namespace Test.It.With.RabbitMQ.NetworkClient
{
    internal class InternalRoutedNetworkClientFactory : INetworkClientFactory, IDisposable
    {
        private readonly InternalRoutedNetworkClient _serverNetworkClient;

        public InternalRoutedNetworkClientFactory(out INetworkClient serverNetworkClient)
        {
            serverNetworkClient = _serverNetworkClient = new InternalRoutedNetworkClient();
        }

        public INetworkClient Create()
        {
            var clientNetworkClient = new InternalRoutedNetworkClient();

            _serverNetworkClient.SendReceived += clientNetworkClient.TriggerReceive;
            _serverNetworkClient.Disconnected += (sender, args) => clientNetworkClient.Dispose();

            clientNetworkClient.SendReceived += _serverNetworkClient.TriggerReceive;
            clientNetworkClient.Disconnected += (sender, args) =>
            {
                _serverNetworkClient.SendReceived -= clientNetworkClient.TriggerReceive;
            };

            return clientNetworkClient;
        }

        public void Dispose()
        {
            _serverNetworkClient.Dispose();
        }
    }
}