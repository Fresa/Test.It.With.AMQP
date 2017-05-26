using System;
using Test.It.Hosting.A.ConsoleApplication.Consoles;

namespace Test.It.Tests
{
    public class TestConsoleApp
    {
        private static TestConsoleApp _app;
        private readonly SimpleServiceContainer _serviceContainer;

        public TestConsoleApp(Action<IServiceContainer> reconfigurer)
        {
            _serviceContainer = new SimpleServiceContainer();
            _serviceContainer.RegisterSingleton<IConsole, Hosting.A.ConsoleApplication.Consoles.Console>();

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

        public event EventHandler<int> Stopped;

        public static void Main(params string[] args)
        {
            _app = new TestConsoleApp(container => { });
            _app.Start(args);
        }
    }
}