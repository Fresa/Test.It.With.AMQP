using System;
using System.Collections.Generic;
using System.Linq;
using Owin;

namespace Test.It.AppBuilders
{
    public class WindowsServiceApplicationServer : IAppBuilder, IDisposable
    {
        private IWindowsServiceController _server;

        private WindowsServiceApplicationServer(Action<IAppBuilder> startup)
        {
            startup(this);
        }

        private WindowsServiceApplicationServer()
        {

        }

        public static WindowsServiceApplicationServer Create(Action<IAppBuilder> startup)
        {
            return new WindowsServiceApplicationServer(startup);
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            if (middleware.GetType().GetInterfaces().Any(type => type == typeof(IWindowsServiceController)) == false)
            {
                throw new ArgumentException(nameof(middleware) + " must implement " + typeof(IWindowsServiceController).FullName);
            }

            _server = (IWindowsServiceController) middleware;
            _server.Start();
            return this;
        }

        public object Build(Type returnType)
        {
            throw new NotImplementedException();
        }

        public IAppBuilder New()
        {
            return new WindowsServiceApplicationServer();
        }

        public IDictionary<string, object> Properties { get; }

        public void Dispose()
        {
            _server.Stop();
        }
    }
}