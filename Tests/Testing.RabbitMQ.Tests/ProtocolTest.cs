using System.Linq;
using System.Xml;
using Should.Fluent;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class ProtocolTest : XUnit2Specification
    {
        private XmlDocument _definition;
        private Protocol.Protocol _protocol;

        protected override void Given()
        {
            var path = "C:\\Users\\Fresa\\Downloads\\amqp0-9-1\\amqp0-9-1.xml";
            _definition = new XmlDocument();
            _definition.Load(path);
        }

        protected override void When()
        {
            _protocol = new Protocol.Protocol(_definition);
        }

        [Fact]
        public void Test()
        {
            _protocol.Constants.Should().Count.AtLeast(1);
            _protocol.Domains.Should().Count.AtLeast(1);
            _protocol.Domains.Should().Contain.Any(pair => pair.Value.Asserts.Any());
            _protocol.Domains.Should().Contain.Any(pair => pair.Value.Rules.Any());
            _protocol.Domains.Should().Not.Contain.Any(pair => string.IsNullOrEmpty(pair.Key));

            _protocol.Major.Should().Equal(0);
            _protocol.Minor.Should().Equal(9);
            _protocol.Revision.Should().Equal(1);
        }
    }
}