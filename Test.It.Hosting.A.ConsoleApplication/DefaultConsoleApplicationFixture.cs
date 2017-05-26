using Test.It.Hosting.A.ConsoleApplication.Consoles;
using Test.It.Specifications;

namespace Test.It.Hosting.A.ConsoleApplication
{
    public class DefaultConsoleApplicationFixture<TApplicationBuilder> : IConsoleApplicationFixture 
        where TApplicationBuilder : IConsoleApplicationBuilder, new()
    {
        public IConsoleClient Start(ITestConfigurer testConfigurer)
        {
            var applicationBuilder = new TApplicationBuilder();
            var server = ConsoleApplicationTestServer.Create(applicationBuilder.CreateWith(testConfigurer).Start);

            return server.Client;
        }
    }
}