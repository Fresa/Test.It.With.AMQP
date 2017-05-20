using System.Net.Http;
using Testing.Framework.Fixtures;

namespace Testing.Framework.Specifications
{
    public abstract class WebSpecification<TFixture> : IUseFixture<TFixture> 
        where TFixture : IWebHostingFixture, new()
    {
        protected HttpClient Client;

        public void SetFixture(TFixture webHostingFixture)
        {
            Client = webHostingFixture.Start(new IntegrationSpecificationConfigurer(new IntegrationSpecification(Given, When)));

            When();
        }

        protected virtual void Given(IServiceContainer configurer) { }
        protected virtual void When() { }
    }
}