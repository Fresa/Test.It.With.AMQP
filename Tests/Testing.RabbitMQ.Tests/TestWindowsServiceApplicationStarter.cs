using Test.It.Starters;

namespace Test.It.With.RabbitMQ.Tests
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