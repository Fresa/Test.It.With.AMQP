using System;
using System.Threading;
using Test.It.Hosting.A.ConsoleApplication.Consoles;

namespace Test.It.Hosting.A.ConsoleApplication
{
    public abstract class ConsoleApplicationSpecification<TFixture> : IUseFixture<TFixture>
        where TFixture : class, IConsoleApplicationFixture, new()
    {
        private readonly AutoResetEvent _wait = new AutoResetEvent(false);

        /// <summary>
        /// Execution timeout. Defaults to 3 seconds.
        /// </summary>
        protected TimeSpan Timeout { private get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        /// Bootstrap the hosted application and start the test fixture.
        /// </summary>
        /// <param name="consoleApplicationFixture">Console application fixture</param>
        public void SetFixture(TFixture consoleApplicationFixture)
        {
            Client = consoleApplicationFixture.Start(new SimpleTestConfigurer(Given));
            Client.Disconnected += (sender, exitCode) => _wait.Set();

            When();

            Wait();
        }

        private void Wait()
        {
            if (_wait.WaitOne(Timeout) == false)
            {
                throw new TimeoutException($"Waited {Timeout.Seconds} seconds.");
            }
        }
        
        /// <summary>
        /// Client to communicate with the hosted console application.
        /// </summary>
        protected IConsoleClient Client { get; private set; }

        /// <summary>
        /// OBS! <see cref="Client"/> is not ready here since the application is in a startup face where you control the service configuration.
        /// </summary>
        /// <param name="configurer">Service container</param>
        protected virtual void Given(IServiceContainer configurer) { }

        /// <summary>
        /// Application has started, and is reachable through <see cref="Client"/>.
        /// </summary>
        protected virtual void When() { }
    }
}