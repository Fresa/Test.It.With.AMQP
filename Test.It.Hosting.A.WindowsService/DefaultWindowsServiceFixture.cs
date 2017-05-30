using Test.It.Specifications;

namespace Test.It.Hosting.A.WindowsService
{
    public class DefaultWindowsServiceFixture<TApplicationBuilder> : IWindowsServiceFixture 
        where TApplicationBuilder : IWindowsServiceBuilder, new()
    {
        private WindowsServiceTestServer _server;

        public IWindowsServiceClient Start(ITestConfigurer testConfigurer)
        {
            var applicationBuilder = new TApplicationBuilder();
            _server = WindowsServiceTestServer.Create(applicationBuilder.CreateWith(testConfigurer).Start);
            return _server.Client;
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}