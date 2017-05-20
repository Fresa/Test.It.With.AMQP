using SimpleInjector;

namespace Testing.RabbitMQ.Tests
{
    public class TestApplicationSpecification
    {
        private readonly SimpleInjectorDependencyResolver _configurer;

        public TestApplicationSpecification()
        {
            _configurer = new SimpleInjectorDependencyResolver(new Container());
        }

        public SimpleInjectorDependencyResolver Configure()
        {
            return _configurer;
        }

        public void Start()
        {
            _configurer.Resolve<TestApplication>().Main();
        }

        public void Stop()
        {
            
        }
    }
}