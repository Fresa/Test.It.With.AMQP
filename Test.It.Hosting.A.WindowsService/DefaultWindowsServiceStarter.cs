using System;

namespace Test.It.Hosting.A.WindowsService
{
    internal class DefaultWindowsServiceStarter<TClient> : BaseWindowsServiceStarter<TClient>
        where TClient : IWindowsServiceController
    {
        public DefaultWindowsServiceStarter(Action starter, TClient client)
        {
            Client = client;
            Starter = starter;
        }

        protected override TClient Client { get; }

        protected override Action Starter { get; }
    }
}