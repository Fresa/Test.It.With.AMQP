using Test.It.Specifications;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.RabbitMQ.Tests.TestApplication;

namespace Test.It.With.RabbitMQ.Tests
{
    public class TestApplicationBuilder : DefaultWindowsServiceBuilder
    {
        public override IWindowsService Create(ITestConfigurer configurer)
        {
            var testApplicationSpecification = new TestApplicationSpecification();
            testApplicationSpecification.Configure(resolver =>
            {
                resolver.AllowOverridingRegistrations();
                configurer.Configure(resolver);
                resolver.DisallowOverridingRegistrations();
            });
            return new TestConsoleApplicationWrapper(testApplicationSpecification);
        }

        private class TestConsoleApplicationWrapper : IWindowsService
        {
            private readonly TestApplicationSpecification _app;

            public TestConsoleApplicationWrapper(TestApplicationSpecification app)
            {
                _app = app;
            }
            
            public int Start(params string[] args)
            {
                _app.Start();
                return 0;
            }

            public void Stop()
            {
                _app.Stop();
            }
        }
    }
}