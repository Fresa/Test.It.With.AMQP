using System.Net.Http;
using Test.It.Fixtures;

namespace Test.It.Specifications
{
    public abstract class WebSpecification<TFixture> : IUseFixture<TFixture> 
        where TFixture : IWebApplicationFixture, new()
    {
        protected HttpClient Client;

        public void SetFixture(TFixture webHostingFixture)
        {
            Client = webHostingFixture.Start(new IntegrationSpecificationConfigurer(Given));

            When();
        }

        protected virtual void Given(IServiceContainer configurer) { }
        protected virtual void When() { }
    }
}