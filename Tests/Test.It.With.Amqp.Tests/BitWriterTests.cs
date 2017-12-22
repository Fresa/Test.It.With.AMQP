using System.Collections.Generic;
using FakeItEasy;
using Should.Fluent;
using Test.It.With.Amqp.Protocol;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.Amqp.Tests
{
    public class When_writing_a_truthy_boolean : XUnit2Specification
    {
        private BitWriter _writer;
        private IByteWriter _byteWriter;

        protected override void Given()
        {
            _byteWriter = A.Fake<IByteWriter>();
            
            _writer = new BitWriter(_byteWriter);
        }

        protected override void When()
        {
            _writer.Write(true);
            _writer.Dispose();
        }

        [Fact]
        public void It_should_write_correct_byte()
        {
            A.CallTo(() => _byteWriter.WriteByte(1)).MustHaveHappened();
        }
    }

    public class When_writing_a_falsy_boolean : XUnit2Specification
    {
        private BitWriter _writer;
        private IByteWriter _byteWriter;

        protected override void Given()
        {
            _byteWriter = A.Fake<IByteWriter>();

            _writer = new BitWriter(_byteWriter);
        }

        protected override void When()
        {
            _writer.Write(false);
            _writer.Dispose();
        }

        [Fact]
        public void It_should_write_correct_byte()
        {
            A.CallTo(() => _byteWriter.WriteByte(0)).MustHaveHappened();
        }
    }

    public class When_writing_multiple_booleans : XUnit2Specification
    {
        private BitWriter _writer;
        private IByteWriter _byteWriter;

        protected override void Given()
        {
            _byteWriter = A.Fake<IByteWriter>();

            _writer = new BitWriter(_byteWriter);
        }

        protected override void When()
        {
            _writer.Write(false);
            _writer.Write(true);
            _writer.Write(true);
            _writer.Write(false);
            _writer.Dispose();
        }

        [Fact]
        public void It_should_write_correct_byte()
        {
            A.CallTo(() => _byteWriter.WriteByte(6)).MustHaveHappened();
        }
    }

    public class When_writing_more_than_eight_booleans : XUnit2Specification
    {
        private BitWriter _writer;
        private IByteWriter _byteWriter;

        protected override void Given()
        {
            _byteWriter = A.Fake<IByteWriter>();

            _writer = new BitWriter(_byteWriter);
        }

        protected override void When()
        {
            // 8
            _writer.Write(false); // 0
            _writer.Write(true); // 2
            _writer.Write(true); // 4
            _writer.Write(false); // 0
            _writer.Write(false); // 0
            _writer.Write(true); // 32
            _writer.Write(true); // 64
            _writer.Write(false); // 0

            // 1
            _writer.Write(true); // 1

            _writer.Dispose();
        }

        [Fact]
        public void It_should_write_correct_bytes()
        {
            A.CallTo(() => _byteWriter.WriteByte(102)).MustHaveHappened();
            A.CallTo(() => _byteWriter.WriteByte(1)).MustHaveHappened();
        }
    }

    public class When_flushing_the_writer_after_a_writing : XUnit2Specification
    {
        private BitWriter _writer;
        private IByteWriter _byteWriter;
        private readonly List<byte> _bytesWritten = new List<byte>();

        protected override void Given()
        {
            _byteWriter = A.Fake<IByteWriter>();
            A.CallTo(() => _byteWriter.WriteByte(A<byte>.Ignored)).Invokes((byte b) => _bytesWritten.Add(b));

            _writer = new BitWriter(_byteWriter);
        }

        protected override void When()
        {
            _writer.Write(false);
            _writer.Write(true);
            _writer.Flush();
            _writer.Write(true); 
            
            _writer.Dispose();
        }

        [Fact]
        public void It_should_write_first_byte()
        {
            _bytesWritten[0].Should().Equal((byte)2);
        }

        [Fact]
        public void It_should_write_second_byte()
        {
            _bytesWritten[1].Should().Equal((byte)1);
        }
    }
}