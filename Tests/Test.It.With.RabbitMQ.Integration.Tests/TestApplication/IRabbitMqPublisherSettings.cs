namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal interface IRabbitMqPublisherSettings
    {
        bool EnablePublisherConfirms { get; }
    }
}