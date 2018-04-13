using Should.Fluent;
using Test.It.With.RabbitMQ._091;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_reading_rabbitmq_qpid_array_field_value_via_amqp : XUnit2Specification
    {
        private RabbitMQ091Reader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new RabbitMQ091Reader(new byte[] { (byte)'x', 0, 0, 0, 5, 0, 6, 1, 2, 3, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            ((ByteArray)_readData).Bytes.Should().Equal(new byte[] { 0, 6, 1, 2, 3 });
        }
    }
}