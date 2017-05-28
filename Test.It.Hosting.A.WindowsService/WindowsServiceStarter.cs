using System;
using Test.It.AppBuilders;
using Test.It.Starters;

namespace Test.It.Hosting.A.WindowsService
{
    public abstract class BaseWindowsServiceStarter<TClient> : IApplicationStarter<TClient>
        where TClient : IWindowsServiceClient
    {
        protected abstract TClient Client { get; }
        protected abstract Action Starter { get; }

        public void Start(IAppBuilder<TClient> applicationBuilder)
        {
            applicationBuilder.WithClient(Client).Use(new TaskStartingMiddleware(Starter));
        }
    }
}