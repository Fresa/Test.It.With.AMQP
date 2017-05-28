using Should.Fluent;
using Xunit;

namespace Test.It.Hosting.A.ConsoleApplication.Tests
{
    public class When_testing_a_console_application : XUnitConsoleApplicationSpecification<DefaultConsoleApplicationFixture<TestConsoleApplicationBuilder>>
    {
        private string _output;
        
        protected override void When()
        {
            Client.OutputReceived += (sender, message) => _output = message;
            Client.Input("test");
        }

        [Fact]
        public void It_should_have_responded()
        {
            _output.Should().Equal("test");
        }
    }
}