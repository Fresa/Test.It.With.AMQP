using Test.It.Hosting.A.WindowsService;
using Test.It.Specifications;

namespace Test.It.With.RabbitMQ.Tests
{
    public class TestApplicationBuilder : DefaultWindowsServiceBuilder
    {
        public override IWindowsService Create(ITestConfigurer configurer)
        {
            var testApplicationSpecification = new TestApplicationSpecification();
            var resolver = testApplicationSpecification.Configure();
            resolver.AllowOverridingRegistrations();
            configurer.Configure(resolver);
            resolver.DisallowOverridingRegistrations();

            return new TestConsoleApplicationWrapper(testApplicationSpecification);
        }

        private class TestConsoleApplicationWrapper : IWindowsService
        {
            private readonly TestApplicationSpecification _app;

            public TestConsoleApplicationWrapper(TestApplicationSpecification app)
            {
                _app = app;
            }

            public void Start()
            {
                _app.Start();
            }

            public void Stop()
            {
                _app.Stop();
            }
        }
    }
}