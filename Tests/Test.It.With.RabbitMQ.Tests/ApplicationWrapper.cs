using System;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.RabbitMQ.Tests.TestApplication;

namespace Test.It.With.RabbitMQ.Tests
{
    internal class ApplicationWrapper : IWindowsService
    {
        private readonly IApplication _app;
        private bool _stopping;

        public ApplicationWrapper(IApplication app)
        {
            _app = app;
            app.OnUnhandledException += exception =>
            {
                OnUnhandledException?.Invoke(exception);
            };
        }

        public int Start(params string[] args)
        {
            _app.Start(args);
            return 0;
        }

        public int Stop()
        {
            if (_stopping)
            {
                return 0;
            }
            _stopping = true;
            _app.Stop();
            return 0;
        }

        public event Action<Exception> OnUnhandledException;
    }
}