namespace Test.It.With.RabbitMQ.NetworkClient
{
    internal interface INetworkClientFactory
    {
        INetworkClient Create();
    }
}