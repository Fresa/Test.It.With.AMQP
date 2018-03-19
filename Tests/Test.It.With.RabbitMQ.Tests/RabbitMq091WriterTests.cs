using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Should.Fluent;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_writing_rabbitmq_qpid_array_field_value_via_amqp : XUnit2Specification
    {
        private RabbitMq091Writer _reader;
        private readonly byte[] _buffer = new byte[10];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new RabbitMq091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(new ByteArray(new byte[] { 0, 6, 1, 2, 3 }));
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'x', 0, 0, 0, 5, 0, 6, 1, 2, 3 });
        }
    }
}