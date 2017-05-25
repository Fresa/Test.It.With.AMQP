using System;
using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.ApplicationBuilders
{
    public abstract class DefaultConsoleApplicationBuilder : IConsoleApplicationBuilder<IServerConsoleClient>
    {
        private static readonly TestConsole TestConsole = new TestConsole();
        private readonly ConsoleClient _consoleClient = new ConsoleClient(TestConsole);

        protected IConsole Console => TestConsole;

        protected void Disconnect(int exitCode)
        {
            TestConsole.Disconnect(exitCode);
        }

        public abstract Func<int> CreateStarter(ITestConfigurer configurer);

        public IApplicationStarter<IServerConsoleClient> CreateWith(ITestConfigurer configurer)
        {
            return new DefaultConsoleApplicationStarter<IServerConsoleClient>(CreateStarter(configurer), _consoleClient);
        }
    }
}