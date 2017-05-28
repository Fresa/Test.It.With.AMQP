namespace Test.It.Hosting.A.WindowsService.Tests
{
    public class When_testing_a_windows_service: XUnitWindowsServiceSpecification<DefaultWindowsServiceFixture<TestWindowsServiceBuilder>>
    {
        protected override void Given(IServiceContainer configurer)
        {

            base.Given(configurer);
        }

        protected override void When()
        {
            base.When();
        }
    }
}