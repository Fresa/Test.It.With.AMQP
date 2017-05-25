using System;
using Test.It.AppBuilders;
using Test.It.Middleware;

namespace Test.It.Starters
{
    public abstract class ConsoleApplicationStarter : IApplicationStarter<IConsoleClient>
    {
        protected abstract IConsoleClient GetClient();
        protected abstract Action Starter { get; }

        public void Start(IAppBuilder<IConsoleClient> applicationBuilder)
        {
            applicationBuilder.WithClient(GetClient()).Use(new TaskStartingMiddleware(Starter));
        }        
    }
}