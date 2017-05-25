using Test.It.ApplicationBuilders;
using Test.It.MessageClient;
using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.Tests
{
    public class TestConsoleApplicationBuilder : IApplicationBuilder<IConsoleClient>
    {
        public IApplicationStarter<IConsoleClient> CreateWith(ITestConfigurer configurer)
        {
            var console = new TestConsole();

            var app = new TestConsoleApp(container =>
            {
                container.Register<IConsole>(() => console);
                configurer.Configure(container);
            });
            return new TestConsoleApplicationStarter(app, console);
        }
    }
}