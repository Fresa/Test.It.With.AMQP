using System;
using Test.It.MessageClient;
using Test.It.Starters;

namespace Test.It.Tests
{
    public class TestConsoleApplicationStarter : ConsoleApplicationStarter
    {
        private readonly ConsoleClient _console;

        public TestConsoleApplicationStarter(TestConsoleApp app, TestConsole console)
        {
            _console = new ConsoleClient(console);
            Starter = () =>
            {
                var exitCode = app.Start();
                console.Disconnect(exitCode);
            };
        }

        protected override IConsoleClient GetClient()
        {
            return _console;
        }

        protected override Action Starter { get; }
    }
}