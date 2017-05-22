namespace Testing.RabbitMQ.NetworkClient
{
    internal class InternalRoutedNetworkClientFactory : INetworkClientFactory
    {
        private readonly InternalRoutedNetworkClient _serverNetworkClient;

        public InternalRoutedNetworkClientFactory()
        {
            _serverNetworkClient = new InternalRoutedNetworkClient();
        }

        public INetworkClient ServerNetworkClient => _serverNetworkClient;

        public INetworkClient Create()
        {
            var clientNetworkClient = new InternalRoutedNetworkClient();

            clientNetworkClient.SendReceived += _serverNetworkClient.TriggerReceive;
            _serverNetworkClient.SendReceived += clientNetworkClient.TriggerReceive;

            return clientNetworkClient;
        }
    }
}