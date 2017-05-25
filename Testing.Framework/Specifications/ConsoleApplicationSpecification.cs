using Test.It.Fixtures;

namespace Test.It.Specifications
{
    public abstract class ConsoleApplicationSpecification<TFixture> : IUseFixture<TFixture>
        where TFixture : class, IConsoleApplicationFixture, new()
    {
        public void SetFixture(TFixture applicationFixture)
        {
            Client = applicationFixture.Start(new IntegrationSpecificationConfigurer(Given));

            When();
        }

        protected IConsoleClient Client { get; private set; }

        protected virtual void Given(IServiceContainer configurer) { }
        protected virtual void When() { }
    }
}