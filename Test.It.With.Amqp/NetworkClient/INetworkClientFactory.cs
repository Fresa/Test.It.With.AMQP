namespace Test.It.With.Amqp.NetworkClient
{
    public interface INetworkClientFactory
    {
        INetworkClient Create();
    }
}