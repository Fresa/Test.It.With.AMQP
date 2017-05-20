using Testing.Framework.Fixtures;

namespace Testing.Framework.Specifications
{
    public abstract class ConsoleApplicationSpecification<TFixture> : IUseFixture<TFixture>
        where TFixture : IConsoleApplicationFixture, new()
    {
        public void SetFixture(TFixture applicationFixture)
        {
            Client = applicationFixture.Start(new IntegrationSpecificationConfigurer(new IntegrationSpecification(Given, When)));

            When();
        }

        protected IClient Client { get; private set; }

        protected virtual void Given(IServiceContainer configurer) { }
        protected virtual void When() { }
    }
}