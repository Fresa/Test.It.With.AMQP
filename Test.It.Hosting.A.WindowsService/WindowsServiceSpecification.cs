using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Test.It.Hosting.A.WindowsService
{
    public abstract class WindowsServiceSpecification<TFixture> : IUseFixture<TFixture>
        where TFixture : class, IWindowsServiceFixture, new()
    {
        private readonly AutoResetEvent _wait = new AutoResetEvent(false);
        private readonly List<Exception> _exceptions = new List<Exception>();

        /// <summary>
        /// Execution timeout. Defaults to 3 seconds.
        /// </summary>
        protected TimeSpan Timeout { private get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        /// Bootstrap the hosted application and start the test fixture.
        /// </summary>
        /// <param name="windowsServiceFixture">Windows service fixture</param>
        public void SetFixture(TFixture windowsServiceFixture)
        {
            var controller = windowsServiceFixture.Start(new SimpleTestConfigurer(Given));
            Client = controller.Client;

            controller.Disconnected += (sender, exitCode) =>
            {
                _wait.Set();
            };
            controller.OnException += (sender, exception) =>
            {
                var aggregateException = exception as AggregateException;
                if (aggregateException == null)
                {
                    _exceptions.Add(exception);
                    return;
                }

                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    _exceptions.Add(innerException);
                }
            };

            When();

            Wait();
        }

        private void Wait()
        {
            if (_wait.WaitOne(Timeout) == false)
            {
                throw new TimeoutException($"Waited {Timeout.Seconds} seconds.");
            }

            HandleExceptions();
        }

        private void HandleExceptions()
        {
            if (_exceptions.Any() == false)
            {
                return;
            }

            if (_exceptions.Count == 1)
            {
                throw _exceptions.First();
            }

            throw new AggregateException(_exceptions);
        }

        /// <summary>
        /// Client to communicate with the hosted windows service application.
        /// </summary>
        protected IWindowsServiceClient Client { get; private set; }

        /// <summary>
        /// OBS! <see cref="Client"/> is not ready here since the application is in bootstrapping phase where you control the service configuration.
        /// </summary>
        /// <param name="configurer">Service container</param>
        protected virtual void Given(IServiceContainer configurer) { }

        /// <summary>
        /// Application has started, and is reachable through <see cref="Client"/>.
        /// </summary>
        protected virtual void When() { }
    }
}