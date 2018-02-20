using System.Collections.Generic;
using FakeItEasy;
using Should.Fluent;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.Amqp.Protocol.Tests
{
    namespace Given_a_bit_reader
    {
        public class When_reading_multiple_bits : XUnit2Specification
        {
            private IByteReader _byteReader;
            private BitReader _reader;
            private readonly List<bool> _bitsRead = new List<bool>();

            protected override void Given()
            {
                _byteReader = A.Fake<IByteReader>();
                var bytesToRead = new Queue<byte>(new[] { (byte)5, (byte)7 });
                A.CallTo(() => _byteReader.ReadByte()).ReturnsLazily(() => bytesToRead.Dequeue());

                _reader = new BitReader(_byteReader);
            }

            protected override void When()
            {
                for (var i = 0; i < 16; i++)
                {
                    _bitsRead.Add(_reader.Read());
                }
            }

            [Fact]
            public void It_should_have_read_correct_bits_for_the_first_byte()
            {
                _bitsRead[0].Should().Be.True();
                _bitsRead[1].Should().Be.False();
                _bitsRead[2].Should().Be.True();
                _bitsRead[3].Should().Be.False();
                _bitsRead[4].Should().Be.False();
                _bitsRead[5].Should().Be.False();
                _bitsRead[6].Should().Be.False();
                _bitsRead[7].Should().Be.False();
            }

            [Fact]
            public void It_should_have_read_correct_bits_for_the_second_byte()
            {
                _bitsRead[8].Should().Be.True();
                _bitsRead[9].Should().Be.True();
                _bitsRead[10].Should().Be.True();
                _bitsRead[11].Should().Be.False();
                _bitsRead[12].Should().Be.False();
                _bitsRead[13].Should().Be.False();
                _bitsRead[14].Should().Be.False();
                _bitsRead[15].Should().Be.False();
            }
        }
    }
}