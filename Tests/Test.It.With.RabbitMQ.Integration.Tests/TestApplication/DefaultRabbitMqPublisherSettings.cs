namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal class DefaultRabbitMqPublisherSettings : IRabbitMqPublisherSettings
    {
        public bool EnablePublisherConfirms { get; set; }
    }
}