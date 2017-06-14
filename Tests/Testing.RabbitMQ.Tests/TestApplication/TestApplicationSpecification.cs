using System.Text;
using RabbitMQ.Client;
using SimpleInjector;

namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    public class TestApplicationSpecification
    {
        private SimpleInjectorDependencyResolver _configurer;
        private IMessagePublisher _messagePublisher;

        public SimpleInjectorDependencyResolver Configure()
        {
            var container = new Container();
            container.RegisterSingleton<IConnectionFactory, ConnectionFactory>();
            container.RegisterSingleton<ISerializer>(() => new NewtonsoftSerializer(Encoding.UTF8));
            container.RegisterSingleton<IMessagePublisherFactory, RabbitMqMessagePublisherFactory>();

            _configurer = new SimpleInjectorDependencyResolver(container);
            return _configurer;
        }

        public void Start()
        {
            _configurer.Verify();

            var messagePublisherFactory = _configurer.Resolve<IMessagePublisherFactory>();
            _messagePublisher = messagePublisherFactory.Create("myExchange");

            _messagePublisher.Publish("myMessage", new TestMessage("Testing sending a message using RabbitMQ"));
        }

        public void Stop()
        {
            _messagePublisher.Dispose();
            _configurer.Dispose();
        }
    }
}