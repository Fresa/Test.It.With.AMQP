using Test.It.AppBuilders;
using Test.It.ApplicationBuilders;
using Test.It.MessageClient;
using Test.It.Specifications;

namespace Test.It.Fixtures
{
    public class StandardConsoleApplicationFixture<TApplicationBuilder> : IConsoleApplicationFixture 
        where TApplicationBuilder : IApplicationBuilder, new()
    {
        public ITypedMessageClient<string, string> Start(ITestConfigurer testConfigurer)
        {
            var applicationBuilder = new TApplicationBuilder();
            var server = ConsoleApplicationTestServer.Create(applicationBuilder.CreateWith(testConfigurer).Start);

            return server.Client;
        }
    }
}