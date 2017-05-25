using System;
using Test.It.Starters;

namespace Test.It
{
    internal class DefaultConsoleApplicationStarter<TConsoleClient> : BaseConsoleApplicationStarter<TConsoleClient> 
        where TConsoleClient : IServerConsoleClient
    {
        private readonly TConsoleClient _console;

        public DefaultConsoleApplicationStarter(Func<int> app, TConsoleClient console)
        {
            _console = console;
            Starter = () =>
            {
                var exitCode = app();
                console.Disconnect(exitCode);
            };
        }

        protected override TConsoleClient GetClient()
        {
            return _console;
        }

        protected override Action Starter { get; }
    }
}