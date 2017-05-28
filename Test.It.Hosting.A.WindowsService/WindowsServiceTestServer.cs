using System;
using System.Collections.Generic;
using Microsoft.Owin.Builder;
using Test.It.AppBuilders;

namespace Test.It.Hosting.A.WindowsService
{
    public class WindowsServiceTestServer : IDisposable
    {
        private WindowsServiceTestServer(Action<IAppBuilder<IWindowsServiceClient>> startup)
        {
            var appBuilder = new AppBuilder();
            var builder = new ClientProvidingAppBuilder<IWindowsServiceClient>(appBuilder);
            startup(builder);
            Client = builder.Client;
            appBuilder.Build()(new Dictionary<string, object>());
        }

        public IWindowsServiceClient Client { get; }

        public static WindowsServiceTestServer Create(Action<IAppBuilder<IWindowsServiceClient>> startup)
        {
            return new WindowsServiceTestServer(startup);
        }

        public void Dispose()
        {
        }
    }
}