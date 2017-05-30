using FakeItEasy;
using Should.Fluent;
using Xunit;

namespace Test.It.Hosting.A.WindowsService.Tests
{
    public class When_testing_a_windows_service : XUnitWindowsServiceSpecification<DefaultWindowsServiceFixture<TestWindowsServiceBuilder>>
    {
        private bool _started;

        protected override void Given(IServiceContainer configurer)
        {
            var app = FakeItEasy.A.Fake<ITestApp>();
            FakeItEasy.A.CallToSet(() => app.HaveStarted).To(true).Invokes(() => _started = true);
            configurer.Register(() => app);
        }
        
        [Fact]
        public void It_should_have_started_the_app()
        {
            _started.Should().Be.True();
        }
    }
}