using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.Builder;
using Test.It.AppBuilders;

namespace Test.It.Hosting.A.WindowsService
{
    public class WindowsServiceTestServer : IDisposable
    {
        private readonly Task _task;

        private WindowsServiceTestServer(Action<IAppBuilder<IWindowsServiceController>> startup)
        {
            var appBuilder = new AppBuilder();
            var builder = new ControllerProvidingAppBuilder<IWindowsServiceController>(appBuilder);
            startup(builder);
            Controller = builder.Controller;
            _task = appBuilder.Build()(new Dictionary<string, object>());
            _task.ContinueWith(task =>
            {
                Controller.RaiseException(task.Exception);
                Controller.Client.Disconnect();
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public IWindowsServiceController Controller { get; }

        public static WindowsServiceTestServer Create(Action<IAppBuilder<IWindowsServiceController>> startup)
        {
            return new WindowsServiceTestServer(startup);
        }

        public void Dispose()
        {
            _task.Dispose();
        }
    }
}