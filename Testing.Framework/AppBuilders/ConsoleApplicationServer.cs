using System;
using System.Collections.Generic;
using System.Linq;
using Owin;
using Testing.Framework.Fixtures;

namespace Testing.Framework.AppBuilders
{
    public class ConsoleApplicationServer : IAppBuilder, IDisposable
    {
        public IClient Client { get; private set; }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            if (middleware.GetType().GetInterfaces().Any(type => type == typeof(IClient)) == false)
            {
                throw new ArgumentException($"{nameof(middleware)} must implement {typeof(IClient).FullName}");
            }

            Client = (IClient)middleware;
            return this;
        }

        private ConsoleApplicationServer(Action<IAppBuilder> startup)
        {
            startup(this);
        }

        private ConsoleApplicationServer()
        {

        }

        public static ConsoleApplicationServer Create(Action<IAppBuilder> startup)
        {
            return new ConsoleApplicationServer(startup);
        }

        public object Build(Type returnType)
        {
            throw new NotImplementedException();
        }

        public IAppBuilder New()
        {
            return new ConsoleApplicationServer();
        }

        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}