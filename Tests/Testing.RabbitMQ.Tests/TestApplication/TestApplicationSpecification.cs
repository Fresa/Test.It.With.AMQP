using System;
using System.Text;
using RabbitMQ.Client;
using SimpleInjector;

namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    public class TestApplicationSpecification
    {
        private SimpleInjectorDependencyResolver _configurer;

        public void Configure(Action<SimpleInjectorDependencyResolver> reconfigurer)
        {
            var container = new Container();
            container.RegisterSingleton<IConnectionFactory, ConnectionFactory>();
            container.RegisterSingleton<ISerializer>(() => new NewtonsoftSerializer(Encoding.UTF8));
            container.RegisterSingleton<IMessagePublisherFactory, RabbitMqMessagePublisherFactory>();

            _configurer = new SimpleInjectorDependencyResolver(container);
            reconfigurer(_configurer);
            _configurer.Verify();
        }

        public void Start()
        {
            var messagePublisherFactory = _configurer.Resolve<IMessagePublisherFactory>();

            using (var messagePublisher = messagePublisherFactory.Create("myExchange"))
            {
                messagePublisher.Publish("myMessage", new TestMessage("Testing sending a message using RabbitMQ"));
            }
        }

        public void Stop()
        {
            _configurer.Dispose();
        }
    }
}