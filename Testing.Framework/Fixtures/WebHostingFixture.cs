using System.Net.Http;
using Microsoft.Owin.Testing;
using Testing.Framework.ApplicationBuilders;
using Testing.Framework.Specifications;

namespace Testing.Framework.Fixtures
{
    public class WebHostingFixture<TApplicationBuilder> : IWebHostingFixture 
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