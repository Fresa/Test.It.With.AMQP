using System.Collections.Generic;
using FakeItEasy;
using Should.Fluent;
using Test.It.With.Amqp.Protocol;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.Amqp.Tests
{
    public class When_reading_a_truthy_boolean : XUnit2Specification
    {
        private BitReader _reader;
        private IByteReader _byteReader;
        private bool _value;

        protected override void Given()
        {
            _byteReader = A.Fake<IByteReader>();
            A.CallTo(() => _byteReader.ReadByte()).Returns((byte)1);

            _reader = new BitReader(_byteReader);
        }

        protected override void When()
        {
            _value = _reader.Read();
        }

        [Fact]
        public void It_should_read_correct_value()
        {
            _value.Should().Be.True();
        }
    }

    public class When_reading_a_falsy_boolean : XUnit2Specification
    {
        private BitReader _reader;
        private IByteReader _byteReader;
        private bool _value;

        protected override void Given()
        {
            _byteReader = A.Fake<IByteReader>();
            A.CallTo(() => _byteReader.ReadByte()).Returns((byte)0);

            _reader = new BitReader(_byteReader);
        }

        protected override void When()
        {
            _value = _reader.Read();
        }

        [Fact]
        public void It_should_read_correct_value()
        {
            _value.Should().Be.False();
        }
    }

    public class When_reading_multiple_booleans : XUnit2Specification
    {
        private BitReader _reader;
        private IByteReader _byteReader;
        private bool _value1;
        private bool _value2;
        private bool _value3;
        private bool _value4;

        protected override void Given()
        {
            _byteReader = A.Fake<IByteReader>();
            A.CallTo(() => _byteReader.ReadByte()).Returns((byte)6);

            _reader = new BitReader(_byteReader);
        }

        protected override void When()
        {
            _value1 = _reader.Read();
            _value2 = _reader.Read();
            _value3 = _reader.Read();
            _value4 = _reader.Read();
        }

        [Fact]
        public void It_should_read_correct_first_value()
        {
            _value1.Should().Be.False();
        }

        [Fact]
        public void It_should_read_correct_second_value()
        {
            _value2.Should().Be.True();
        }

        [Fact]
        public void It_should_read_correct_third_value()
        {
            _value3.Should().Be.True();
        }

        [Fact]
        public void It_should_read_correct_fourth_value()
        {
            _value4.Should().Be.False();
        }
    }

    public class When_reading_more_than_eight_booleans : XUnit2Specification
    {
        private BitReader _reader;
        private IByteReader _byteReader;
        private bool _value1;
        private bool _value2;
        private bool _value3;
        private bool _value4;
        private bool _value5;
        private bool _value6;
        private bool _value7;
        private bool _value8;
        private bool _value9;

        protected override void Given()
        {
            var bytes = new Queue<int>(new[] { 102, 1 });
            _byteReader = A.Fake<IByteReader>();
            A.CallTo(() => _byteReader.ReadByte()).ReturnsLazily(() => (byte)bytes.Dequeue());

            _reader = new BitReader(_byteReader);
        }

        protected override void When()
        {
            // 8
            _value1 = _reader.Read();
            _value2 = _reader.Read();
            _value3 = _reader.Read();
            _value4 = _reader.Read();
            _value5 = _reader.Read();
            _value6 = _reader.Read();
            _value7 = _reader.Read();
            _value8 = _reader.Read();

            // 1
            _value9 = _reader.Read();
        }

        [Fact]
        public void It_should_read_correct_first_value()
        {
            _value1.Should().Be.False();
        }

        [Fact]
        public void It_should_read_correct_second_value()
        {
            _value2.Should().Be.True();
        }

        [Fact]
        public void It_should_read_correct_third_value()
        {
            _value3.Should().Be.True();
        }

        [Fact]
        public void It_should_read_correct_fourth_value()
        {
            _value4.Should().Be.False();
        }

        [Fact]
        public void It_should_read_correct_fifth_value()
        {
            _value5.Should().Be.False();
        }

        [Fact]
        public void It_should_read_correct_sixth_value()
        {
            _value6.Should().Be.True();
        }

        [Fact]
        public void It_should_read_correct_seventh_value()
        {
            _value7.Should().Be.True();
        }

        [Fact]
        public void It_should_read_correct_eigth_value()
        {
            _value8.Should().Be.False();
        }

        [Fact]
        public void It_should_read_correct_ninth_value()
        {
            _value9.Should().Be.True();
        }
    }

    public class When_reseting_the_reader_after_a_read : XUnit2Specification
    {
        private IByteReader _byteReader;
        private BitReader _reader;
        private bool _value1;
        private bool _value2;

        protected override void Given()
        {
            var bytes = new Queue<int>(new[] { 102, 0 });
            _byteReader = A.Fake<IByteReader>();
            A.CallTo(() => _byteReader.ReadByte()).ReturnsLazily(() => (byte)bytes.Dequeue());

            _reader = new BitReader(_byteReader);
        }

        protected override void When()
        {
            _value1 = _reader.Read();
            _reader.Reset();
            _value2 = _reader.Read();
        }

        [Fact]
        public void It_should_have_read_the_first_value_from_the_first_byte()
        {
            _value1.Should().Be.False();
        }

        [Fact]
        public void It_should_have_read_the_second_value_from_the_second_byte()
        {
            _value2.Should().Be.False();
        }

        [Fact]
        public void It_should_have_read_two_bytes()
        {
            A.CallTo(() => _byteReader.ReadByte()).MustHaveHappened(Repeated.Exactly.Twice);
        }
    }
}