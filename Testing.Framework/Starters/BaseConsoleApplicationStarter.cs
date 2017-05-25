using System;
using Test.It.AppBuilders;
using Test.It.Middleware;

namespace Test.It.Starters
{
    public abstract class BaseConsoleApplicationStarter<TConsoleClient> : IApplicationStarter<TConsoleClient>
        where TConsoleClient : IConsoleClient
    {
        protected abstract TConsoleClient GetClient();
        protected abstract Action Starter { get; }

        public void Start(IAppBuilder<TConsoleClient> applicationBuilder)
        {
            applicationBuilder.WithClient(GetClient()).Use(new TaskStartingMiddleware(Starter));
        }        



    }
}