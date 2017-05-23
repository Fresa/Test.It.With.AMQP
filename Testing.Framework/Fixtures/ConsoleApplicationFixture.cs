using Test.It.AppBuilders;
using Test.It.ApplicationBuilders;
using Test.It.Specifications;

namespace Test.It.Fixtures
{
    public class ConsoleApplicationFixture<TApplicationBuilder> : IConsoleApplicationFixture 
        where TApplicationBuilder : IApplicationBuilder, new()
    {
        public IClient Start(ITestConfigurer testConfigurer)
        {
            var applicationBuilder = new TApplicationBuilder();
            var server = ConsoleApplicationServer.Create(applicationBuilder.CreateWith(testConfigurer).Start);

            return server.Client;
        }
    }
}