using System.Text;
using RabbitMQ.Client;
using SimpleInjector;

namespace Test.It.With.RabbitMQ.Tests
{
    public class TestApplicationSpecification
    {
        private SimpleInjectorDependencyResolver _configurer;
        private TestApplication _application;

        public SimpleInjectorDependencyResolver Configure()
        {
            var container = new Container();
            container.RegisterSingleton<IConnectionFactory, ConnectionFactory>();
            container.RegisterSingleton<ISerializer>(() => new NewtonsoftSerializer(Encoding.UTF8));

            _configurer = new SimpleInjectorDependencyResolver(container);
            return _configurer;
        }

        public IMessageQueueApplication Start()
        {
            _configurer.Verify();

            _application = _configurer.Resolve<TestApplication>();
            _application.Start();

            return _application;
        }

        public void Stop()
        {
            _application.Dispose();
            _configurer.Dispose();
        }
    }
}