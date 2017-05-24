using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Builder;
using Owin;
using Test.It.MessageClient;

namespace Test.It.AppBuilders
{
    public class ConsoleApplicationTestServer : IDisposable
    {
        private ConsoleApplicationTestServer(Action<IAppBuilder> startup)
        {
            var builder = new MyAppBuilder(new AppBuilder());
            startup(builder);
            Client = builder.Client;
            builder.Build()(null);
        }

        public ITypedMessageClient<string, string> Client { get; }

        public static ConsoleApplicationTestServer Create(Action<IAppBuilder> startup)
        {
            return new ConsoleApplicationTestServer(startup);
        }
        
        public void Dispose()
        {
            //Client.Dispose();
        }

        
    }

    public interface IAppBuilder<in T>
    {
        IAppBuilder Use(T middleware, params object[] args);
    }

    public class MyAppBuilder : IAppBuilder
    {
        private readonly IAppBuilder _appBuilder;

        public MyAppBuilder(IAppBuilder appBuilder)
        {
            _appBuilder = appBuilder;
        }

        public ITypedMessageClient<string, string> Client { get; private set; }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            Client = (ITypedMessageClient<string, string>) args[0];
            return _appBuilder.Use(middleware);
        }

        public object Build(Type returnType)
        {
            return _appBuilder.Build(returnType);
        }

        public IAppBuilder New()
        {
            return _appBuilder.New();
        }

        public IDictionary<string, object> Properties => _appBuilder.Properties;
    }
}