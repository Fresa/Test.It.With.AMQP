using RabbitMQ.Client;
using Test.It.With.RabbitMQ.NetworkClient;

namespace Test.It.With.RabbitMQ
{
    public static class NetworkClientFactoryExtensions
    {
        public static IConnectionFactory ToRabbitMqConnectionFactory(
            this INetworkClientFactory networkClientFactory)
        {
            return new TestConnectionFactory(networkClientFactory);
        }
    }
}