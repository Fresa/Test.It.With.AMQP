using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Should.Fluent;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.Amqp.Protocol._091.Tests
{
    public class When_writing_a_short_unsigned_integer_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[2];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteShortUnsignedInteger(2);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 0, 2 });
        }
    }

    public class When_writing_a_long_unsigned_integer_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[4];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteLongUnsignedInteger(2);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 0, 0, 0, 2 });
        }
    }

    public class When_writing_a_long_long_unsigned_integer_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[8];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteLongLongUnsignedInteger(2);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0, 2 });
        }
    }

    public class When_writing_a_short_string_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[6];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteShortString("ABCDE");
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 5, 65, 66, 67, 68, 69 });
        }
    }

    public class When_writing_a_byte_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[1];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteByte(16);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 16 });
        }
    }

    public class When_writing_a_boolean_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[1];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteBoolean(true);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 1 });
        }
    }

    public class When_writing_bytes_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[3];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteBytes(new byte[] { 1, 0, 6 });
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 1, 0, 6 });
        }
    }

    public class When_writing_long_string_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[9];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteLongString(new byte[] { 0, 6, 1, 2, 3 });
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 0, 0, 0, 5, 0, 6, 1, 2, 3 });
        }
    }

    public class When_writing_character_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[2];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteCharacter('ऎ');
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 14, 9 });
        }
    }

    public class When_writing_short_short_integer_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[1];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteShortShortInteger(65);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 65 });
        }
    }

    public class When_writing_long_long_integer_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[8];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteLongLongInteger(65);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0, 65 });
        }
    }

    public class When_writing_decimal_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[5];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteDecimal(new decimal(0.04));
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 2, 0, 0, 0, 4 });
        }
    }

    public class When_writing_negative_decimal_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[5];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteDecimal(new decimal(-0.04));
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 2, 255, 255, 255, 252 });
        }
    }

    public class When_writing_long_integer_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[4];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteLongInteger(257);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 0, 0, 1, 1 });
        }
    }

    public class When_writing_short_integer_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[2];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteShortInteger(256);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 1, 0 });
        }
    }

    public class When_writing_a_floating_point_number_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[4];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFloatingPointNumber((float)1.5000000E+001);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 65, 112, 0, 0 });
        }
    }

    public class When_writing_a_long_floating_point_number_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[8];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteLongFloatingPointNumber(2.5500000000000000E+002);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 64, 111, 224, 0, 0, 0, 0, 0 });
        }
    }

    public class When_writing_a_timestamp_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[8];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteTimestamp(new DateTime(1970, 7, 14, 4, 20, 21, DateTimeKind.Utc));
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 0, 0, 0, 0, 1, 0, 0, 5 });
        }
    }

    public class When_writing_a_table_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[12];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteTable(new Dictionary<string, object>{ {"ABCDE", true} });
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { 0, 0, 0, 8, 5, 65, 66, 67, 68, 69, (byte)'t', 1 });
        }
    }

    public class When_writing_boolean_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[2];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(true);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'t', 1 });
        }
    }

    public class When_writing_short_short_integer_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[2];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((sbyte)5);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'b', 5 });
        }
    }

    public class When_writing_a_byte_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[2];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((byte)5);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'B', 5 });
        }
    }

    public class When_writing_a_short_integer_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[3];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((short)262);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'U', 1, 6 });
        }
    }

    public class When_writing_a_short_unsigned_integer_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[3];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((ushort)262);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'u', 1, 6 });
        }
    }

    public class When_writing_a_long_integer_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[5];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(262);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'I', 0, 0, 1, 6 });
        }
    }

    public class When_writing_a_long_unsigned_integer_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[5];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((uint)262);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'i', 0, 0, 1, 6 });
        }
    }

    public class When_writing_a_long_long_integer_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[9];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((long)262);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'L', 0, 0, 0, 0, 0, 0, 1, 6 });
        }
    }

    public class When_writing_a_long_long_unsigned_integer_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[9];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((ulong)262);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'l', 0, 0, 0, 0, 0, 0, 1, 6 });
        }
    }

    public class When_writing_a_floating_point_number_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[5];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((float)1.5000000E+001);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'f', 65, 112, 0, 0 });
        }
    }

    public class When_writing_a_long_floating_point_number_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[9];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(2.5500000000000000E+002);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'d', 64, 111, 224, 0, 0, 0, 0, 0 });
        }
    }

    public class When_writing_a_decimal_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[6];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue((decimal)0.04);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'D', 2, 0, 0, 0, 4 });
        }
    }

    public class When_writing_negative_decimal_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[6];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(new decimal(-0.04));
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'D', 2, 255, 255, 255, 252 });
        }
    }
    
    public class When_writing_a_short_string_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[7];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue("ABCDE");
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'s', 5, 65, 66, 67, 68, 69 });
        }
    }

    public class When_writing_a_long_string_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[10];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(new byte[] { 65, 66, 67, 68, 69 });
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'S', 0, 0, 0, 5, 65, 66, 67, 68, 69 });
        }
    }

    public class When_writing_a_timestamp_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[9];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(new DateTime(1970, 7, 14, 4, 20, 21, DateTimeKind.Utc));
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'T', 0, 0, 0, 0, 1, 0, 0, 5 });
        }
    }

    public class When_writing_a_table_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[13];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(new Dictionary<string, object>{{ "ABCDE", true } });
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'F', 0, 0, 0, 8, 5, 65, 66, 67, 68, 69, (byte)'t', 1 });
        }
    }

    public class When_writing_a_non_generic_table_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[13];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(new HybridDictionary { { "ABCDE", true } });
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'F', 0, 0, 0, 8, 5, 65, 66, 67, 68, 69, (byte)'t', 1 });
        }
    }

    public class When_writing_an_array_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[7];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(new []{true});
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new byte[] { (byte)'A', 0, 0, 0, 2, (byte)'t', 1 });
        }
    }

    public class When_writing_no_field_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[1];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
        }

        protected override void When()
        {
            _reader.WriteFieldValue(null);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _buffer.Should().Equal(new[] { (byte)'V' });
        }
    }

    public class When_writing_rabbitmq_qpid_array_field_value_via_amqp : XUnit2Specification
    {
        private Amqp091Writer _reader;
        private readonly byte[] _buffer = new byte[10];

        protected override void Given()
        {
            var stream = new MemoryStream(_buffer);
            _reader = new Amqp091Writer(stream);
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

    public class When_writing_one_full_property_flag_set : XUnit2Specification
    {
        private bool[] _array;
        private MemoryStream _stream;
        private Amqp091Writer _writer;

        protected override void Given()
        {
            _stream = new MemoryStream();
            _writer = new Amqp091Writer(_stream);
            _array = new[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, true };
        }

        protected override void When()
        {
            _writer.WritePropertyFlags(_array);
        }

        [Fact]
        public void It_should_have_converted_it_correctly()
        {
            _stream.ToArray().Should().Equal(new byte[] { 0, 2 });
        }
    }

    public class When_writing_one_property_flag_set_that_is_not_complete : XUnit2Specification
    {
        private bool[] _array;
        private MemoryStream _stream;
        private Amqp091Writer _writer;

        protected override void Given()
        {
            _stream = new MemoryStream();
            _writer = new Amqp091Writer(_stream);
            _array = new[] { true, false };
        }

        protected override void When()
        {
            _writer.WritePropertyFlags(_array);
        }

        [Fact]
        public void It_should_have_converted_it_correctly()
        {
            _stream.ToArray().Should().Equal(new byte[] { 128, 0 });
        }
    }

    public class When_writing_multiple_property_flag_sets : XUnit2Specification
    {
        private bool[] _array;
        private MemoryStream _stream;
        private Amqp091Writer _writer;

        protected override void Given()
        {
            _stream = new MemoryStream();
            _writer = new Amqp091Writer(_stream);
            _array = new[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false };
        }

        protected override void When()
        {
            _writer.WritePropertyFlags(_array);
        }

        [Fact]
        public void It_should_have_converted_it_correctly()
        {
            _stream.ToArray().Should().Equal(new byte[] { 0, 3, 128, 0 });
        }
    }
}