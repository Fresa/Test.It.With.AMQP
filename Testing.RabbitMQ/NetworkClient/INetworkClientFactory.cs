namespace Testing.RabbitMQ.NetworkClient
{
    internal interface INetworkClientFactory
    {
        INetworkClient Create();
    }
}