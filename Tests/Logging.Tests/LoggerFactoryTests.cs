using Logging.Loggers;
using Should.Fluent;
using Testing.Framework.Specifications;
using Xunit;

namespace Logging.Tests
{
    public class When_creating_a_logger : Specification
    {
        private ILogger _createdLogger;
        
        public When_creating_a_logger()
        {
            Setup();
        }
        
        protected override void When()
        {
            _createdLogger = LoggerFactory.Create<When_creating_a_logger>();
        }
        
        [Fact]
        public void It_should_create_a_fake_logger()
        {
            _createdLogger.Should().Be.OfType<FakeLogger>();
        }
    }
}