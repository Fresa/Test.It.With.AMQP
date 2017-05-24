using Test.It.Fixtures;

namespace Test.It.Specifications
{
    public abstract class WindowsServiceSpecification<TWindowsServiceFixture> : IUseFixture<TWindowsServiceFixture>
        where TWindowsServiceFixture : IWindowsServiceFixture, new()
    {
        public void SetFixture(TWindowsServiceFixture fixture)
        {
            fixture.Start(new IntegrationSpecificationConfigurer(Given));

            When();
        }

        protected virtual void Given(IServiceContainer configurer) { }
        protected virtual void When() { }
    }
}