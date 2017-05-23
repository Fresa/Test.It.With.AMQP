using Test.It.AppBuilders;
using Test.It.ApplicationBuilders;
using Test.It.Specifications;

namespace Test.It.Fixtures
{
    public class WindowsServiceFixture<TApplicationBuilder> : IWindowsServiceFixture 
        where TApplicationBuilder : IApplicationBuilder, new()
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