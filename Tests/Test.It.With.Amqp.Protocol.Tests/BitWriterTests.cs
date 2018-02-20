using FakeItEasy;
using Should.Fluent;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.Amqp.Protocol.Tests
{
    namespace Given_a_bit_writer
    {
        public class When_writing_multiple_bits : XUnit2Specification
        {
            private IByteWriter _byteWriter;
            private BitWriter _writer;
            private byte _byteWritten;

            protected override void Given()
            {
                _byteWriter = A.Fake<IByteWriter>();
                A.CallTo(() => _byteWriter.WriteByte(A<byte>.Ignored))
                    .Invokes((byte @byte) => _byteWritten = @byte);

                _writer = new BitWriter(_byteWriter);
            }

            protected override void When()
            {
                _writer.Write(false);
                _writer.Write(true);
                _writer.Write(false);
                _writer.Write(true);
                _writer.Flush();
            }

            [Fact]
            public void It_should_have_written_correct_byte()
            {
                _byteWritten.Should().Equal((byte) 10);
            }
        }
    }
}