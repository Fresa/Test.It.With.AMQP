using Testing.Framework;
using Testing.Framework.ApplicationBuilders;
using Testing.Framework.Starters;

namespace Testing.RabbitMQ.Tests
{
    public class TestApplicationBuilder : ApplicationBuilder
    {
        private readonly TestApplicationSpecification _testApplicationSpecification;

        public TestApplicationBuilder()
        {
            _testApplicationSpecification = new TestApplicationSpecification();
        }

        protected override IServiceContainer UseServiceContainer()
        {
            return _testApplicationSpecification.Configure();
        }

        protected override IApplicationStarter GetApplicationStarter()
        {
            return new TestWindowsServiceApplicationStarter(_testApplicationSpecification);
        }
    }
}