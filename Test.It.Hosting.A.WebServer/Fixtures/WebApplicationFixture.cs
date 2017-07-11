using System.Net.Http;
using Microsoft.Owin.Testing;
using Test.It.Hosting.A.WebServer.ApplicationBuilders;
using Test.It.Specifications;

namespace Test.It.Hosting.A.WebServer.Fixtures
{
    public class WebApplicationFixture<TApplicationBuilder> : IWebApplicationFixture 
        where TApplicationBuilder : IApplicationBuilder, new()
    {
        private TestServer _testServer;
        
        public HttpClient Start(ITestConfigurer testConfigurer)
        {
            var applicationBuilder = new TApplicationBuilder();
            _testServer = TestServer.Create(applicationBuilder.CreateWith(testConfigurer).Start);

            return _testServer.HttpClient;
        }

        public void Dispose()
        {
            _testServer.Dispose();
        }
    }
}