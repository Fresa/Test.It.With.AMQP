using System;
using Test.It.ApplicationBuilders;
using Test.It.MessageClient;
using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.Tests
{
    public class TestConsoleApplicationBuilder : DefaultConsoleApplicationBuilder
    {
        public override Func<int> CreateStarter(ITestConfigurer configurer)
        {
            var app = new TestConsoleApp(container =>
            {
                container.Register(() => Console);
                configurer.Configure(container);
            });

            return () => app.Start();
        }
    }
}