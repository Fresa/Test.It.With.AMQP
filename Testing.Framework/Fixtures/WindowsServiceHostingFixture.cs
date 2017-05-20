using Testing.Framework.AppBuilders;
using Testing.Framework.ApplicationBuilders;
using Testing.Framework.Specifications;

namespace Testing.Framework.Fixtures
{
    public class WindowsServiceHostingFixture<TApplicationBuilder> : IWindowsServiceFixture where TApplicationBuilder : IApplicationBuilder, new()
    {
        private WindowsServiceApplicationServer _server;

        public void Start(ITestConfigurer testConfigurer)
        {
            var applicationBuilder = new TApplicationBuilder();
            _server = WindowsServiceApplicationServer.Create(applicationBuilder.CreateWith(testConfigurer).Start);
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}