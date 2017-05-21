using Testing.Framework;
using Testing.Framework.ApplicationBuilders;
using Testing.Framework.Starters;

namespace Testing.RabbitMQ.Tests
{
    public class TestApplicationBuilder : ApplicationBuilder
    {
        private readonly TestApplicationSpecification _testApplicationSpecification;
        private SimpleInjectorDependencyResolver _configurer;

        public TestApplicationBuilder()
        {
            _testApplicationSpecification = new TestApplicationSpecification();
        }

        protected override IServiceContainer UseServiceContainer()
        {
            _configurer = _testApplicationSpecification.Configure();
            _configurer.AllowOverridingRegistrations();
            return _configurer;
        }

        protected override IApplicationStarter GetApplicationStarter()
        {
            _configurer.DisallowOverridingRegistrations();
            return new TestWindowsServiceApplicationStarter(_testApplicationSpecification);
        }
    }
}