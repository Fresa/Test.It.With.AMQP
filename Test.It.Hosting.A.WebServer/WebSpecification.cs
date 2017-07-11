using System.Net.Http;
using Test.It.Hosting.A.WebServer.Fixtures;

namespace Test.It.Hosting.A.WebServer
{
    public abstract class WebSpecification<TFixture> : IUseFixture<TFixture> 
        where TFixture : IWebApplicationFixture, new()
    {
        protected HttpClient Client;

        public void SetFixture(TFixture webHostingFixture)
        {
            Client = webHostingFixture.Start(new SimpleTestConfigurer(Given));

            When();
        }

        protected virtual void Given(IServiceContainer configurer) { }
        protected virtual void When() { }
    }
}