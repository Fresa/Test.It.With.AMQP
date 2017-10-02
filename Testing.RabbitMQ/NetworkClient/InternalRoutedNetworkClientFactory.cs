namespace Test.It.With.RabbitMQ.NetworkClient
{
    internal class InternalRoutedNetworkClientFactory : INetworkClientFactory
    {
        private readonly InternalRoutedNetworkClient _serverNetworkClient;

        public InternalRoutedNetworkClientFactory(out INetworkClient serverNetworkClient)
        {
            serverNetworkClient = _serverNetworkClient = new InternalRoutedNetworkClient();
        }

        public INetworkClient Create()
        {
            var clientNetworkClient = new InternalRoutedNetworkClient();

            clientNetworkClient.SendReceived += _serverNetworkClient.TriggerReceive;
            _serverNetworkClient.SendReceived += clientNetworkClient.TriggerReceive;

            return clientNetworkClient;
        }
    }
}