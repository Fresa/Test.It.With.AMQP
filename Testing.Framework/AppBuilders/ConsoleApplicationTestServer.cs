using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Builder;
using Test.It.MessageClient;

namespace Test.It.AppBuilders
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