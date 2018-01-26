using Test.It.Specifications;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.RabbitMQ.Tests.TestApplication;

namespace Test.It.With.RabbitMQ.Tests
{
    public class TestApplicationBuilder<TApplication> : DefaultWindowsServiceBuilder where TApplication : IApplication, new()
    {
        public override IWindowsService Create(ITestConfigurer configurer)
        {
            var testApplicationSpecification = new TApplication();
            testApplicationSpecification.Configure(resolver =>
            {
                resolver.AllowOverridingRegistrations();
                configurer.Configure(resolver);
                resolver.DisallowOverridingRegistrations();
            });
            return new ApplicationWrapper(testApplicationSpecification);
        }
    }
}