using System;
using Test.It.AppBuilders;
using Test.It.Hosting.A.ConsoleApplication.Consoles;
using Test.It.Starters;

namespace Test.It.Hosting.A.ConsoleApplication
{
    public abstract class BaseConsoleApplicationStarter<TConsoleClient> : IApplicationStarter<TConsoleClient>
        where TConsoleClient : IConsoleClient
    {
        protected abstract TConsoleClient Client { get; }
        protected abstract Action Starter { get; }

        public void Start(IAppBuilder<TConsoleClient> applicationBuilder)
        {
            applicationBuilder.WithController(Client).Use(new TaskStartingMiddleware(Starter));
        }        
    }
}