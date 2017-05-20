using Testing.Framework.AppBuilders;
using Testing.Framework.ApplicationBuilders;
using Testing.Framework.Specifications;

namespace Testing.Framework.Fixtures
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