namespace Test.It.With.RabbitMQ.NetworkClient
{
    public interface INetworkClientFactory
    {
        INetworkClient Create();
    }
}