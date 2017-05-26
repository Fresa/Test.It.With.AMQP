using Test.It.Hosting.A.ConsoleApplication.Consoles;
using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.Hosting.A.ConsoleApplication
{
    public abstract class DefaultConsoleApplicationBuilder : IConsoleApplicationBuilder
    {
        private static readonly TestConsole TestConsole = new TestConsole();
        private readonly ConsoleClient _consoleClient = new ConsoleClient(TestConsole);

        public abstract IConsoleApplication Create(ITestConfigurer configurer);
        
        public IApplicationStarter<IConsoleClient> CreateWith(ITestConfigurer configurer)
        {            
            var application = Create(new TestConsoleTestConfigurerDecorator(configurer));

            void InternalStarter()
            {
                var exitCode = application.Start();
                TestConsole.Disconnect(exitCode);
            }

            return new DefaultConsoleApplicationStarter<IConsoleClient>(InternalStarter, _consoleClient);
        }

        private class TestConsoleTestConfigurerDecorator : ITestConfigurer
        {
            private readonly ITestConfigurer _configurer;

            public TestConsoleTestConfigurerDecorator(ITestConfigurer configurer)
            {
                _configurer = configurer;
            }

            public void Configure(IServiceContainer container)
            {
                _configurer.Configure(container);
                container.RegisterSingleton<IConsole>(() => TestConsole);
            }
        }
    }
}