using System;
using Test.It.Hosting.A.ConsoleApplication.Consoles;

namespace Test.It.Hosting.A.ConsoleApplication
{
    internal class DefaultConsoleApplicationStarter<TConsoleClient> : BaseConsoleApplicationStarter<TConsoleClient> 
        where TConsoleClient : IConsoleClient
    {
        public DefaultConsoleApplicationStarter(Action starter, TConsoleClient console)
        {
            Client = console;
            Starter = starter;
        }

        protected override TConsoleClient Client { get; }

        protected override Action Starter { get; }
    }
}