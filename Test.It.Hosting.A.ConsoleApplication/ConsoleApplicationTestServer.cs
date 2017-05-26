using System;
using System.Collections.Generic;
using Microsoft.Owin.Builder;
using Test.It.AppBuilders;
using Test.It.Hosting.A.ConsoleApplication.Consoles;

namespace Test.It.Hosting.A.ConsoleApplication
{
    public class ConsoleApplicationTestServer : IDisposable
    {
        private ConsoleApplicationTestServer(Action<IAppBuilder<IConsoleClient>> startup)
        {
            var appBuilder = new AppBuilder();
            var builder = new ClientProvidingAppBuilder<IConsoleClient>(appBuilder);
            startup(builder);
            Client = builder.Client;
            appBuilder.Build()(new Dictionary<string, object>());
        }

        public IConsoleClient Client { get; }

        public static ConsoleApplicationTestServer Create(Action<IAppBuilder<IConsoleClient>> startup)
        {
            return new ConsoleApplicationTestServer(startup);
        }

        public void Dispose()
        {
        }
    }
}