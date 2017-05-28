using System;
using Test.It.MessageClient;

namespace Test.It.Hosting.A.WindowsService.Tests
{
    public class TestWindowsServiceApp
    {
        private static TestWindowsServiceApp _app;
        private readonly SimpleServiceContainer _serviceContainer;

        public TestWindowsServiceApp(Action<IServiceContainer> reconfigurer)
        {
            _serviceContainer = new SimpleServiceContainer();
            _serviceContainer.RegisterSingleton<IMessageClient>(() => new MessageClient.MessageClient());, Consoles.Console>();

            reconfigurer(_serviceContainer);
            _serviceContainer.Verify();
        }

        public int Start(params string[] args)
        {
            var console = _serviceContainer.Resolve<IConsole>();
            console.WriteLine(console.ReadLine());
            Stopped?.Invoke(this, 0);
            return 0;
        }

        public void Stop()
        {
            
        }

        public event EventHandler<int> Stopped;

        public static void Main(params string[] args)
        {
            _app = new TestWindowsServiceApp(container => { });
            _app.Start(args);
        }
    }
}