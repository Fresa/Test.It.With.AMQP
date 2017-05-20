using Testing.Framework;
using Testing.Framework.Starters;

namespace Testing.RabbitMQ.Tests
{
    public class TestWindowsServiceApplicationStarter : WindowsServiceStarter
    {
        private readonly TestApplicationSpecification _testApplicationSpecification;

        public TestWindowsServiceApplicationStarter(TestApplicationSpecification testApplicationSpecification)
        {
            _testApplicationSpecification = testApplicationSpecification;
        }

        protected override IWindowsServiceController GetServiceController()
        {
            return new TestApplicationController(_testApplicationSpecification);
        }
    }
}