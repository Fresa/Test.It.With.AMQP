using System;
using System.Collections.Generic;
using Should.Fluent;
using Test.It.With.RabbitMQ.MessageClient;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_reading_a_short_unsigned_integer_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private ushort _ushort;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 0, 2 });
        }

        protected override void When()
        {
            _ushort = _reader.ReadShortUnsignedInteger();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _ushort.Should().Equal((ushort)2);
        }
    }

    public class When_reading_a_long_unsigned_integer_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private uint _ushort;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 0, 0, 0, 2 });
        }

        protected override void When()
        {
            _ushort = _reader.ReadLongUnsignedInteger();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _ushort.Should().Equal((uint)2);
        }
    }

    public class When_reading_a_long_long_unsigned_integer_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private ulong _ushort;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 0, 0, 0, 0, 0, 0, 0, 2 });
        }

        protected override void When()
        {
            _ushort = _reader.ReadLongLongUnsignedInteger();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _ushort.Should().Equal((ulong)2);
        }
    }

    public class When_reading_a_short_string_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private string _parsedData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 5, 65, 66, 67, 68, 69, 6 });
        }

        protected override void When()
        {
            _parsedData = _reader.ReadShortString();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _parsedData.Should().Equal("ABCDE");
        }
    }

    public class When_reading_a_byte_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private byte _parsedData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 16, 14 });
        }

        protected override void When()
        {
            _parsedData = _reader.ReadByte();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _parsedData.Should().Equal((byte)16);
        }
    }

    public class When_reading_a_boolean_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private bool _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 1, 0 });
        }

        protected override void When()
        {
            _readData = _reader.ReadBoolean();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(true);
        }
    }

    public class When_reading_bytes_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private byte[] _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 1, 0, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadBytes(2);
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(new byte[] { 1, 0 });
        }
    }

    public class When_reading_long_string_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private byte[] _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 0, 0, 0, 5, 0, 6, 1, 2, 3, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadLongString();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(new byte[] { 0, 6, 1, 2, 3 });
        }
    }

    public class When_reading_character_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private char _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 14, 9, 5 });
        }

        protected override void When()
        {
            _readData = _reader.ReadCharacter();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal('ऎ');
        }
    }

    public class When_reading_short_short_integer_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private sbyte _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 65 });
        }

        protected override void When()
        {
            _readData = _reader.ReadShortShortInteger();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((sbyte)65);
        }
    }

    public class When_reading_long_long_integer_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private long _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 0,0,0,0,0,0,0,65 });
        }

        protected override void When()
        {
            _readData = _reader.ReadLongLongInteger();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((long)65);
        }
    }

    public class When_reading_decimal_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private decimal _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 2, 0, 0, 0, 4, 5 });
        }

        protected override void When()
        {
            _readData = _reader.ReadDecimal();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((decimal)0.04);
        }
    }

    public class When_reading_long_integer_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private int _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 0, 0, 1, 1, 4, 5 });
        }

        protected override void When()
        {
            _readData = _reader.ReadLongInteger();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(257);
        }
    }

    public class When_reading_short_integer_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private int _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 1, 0, 1, 1, 4, 5 });
        }

        protected override void When()
        {
            _readData = _reader.ReadShortInteger();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(256);
        }
    }

    public class When_reading_a_floating_point_number_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private float _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 65, 112, 0, 0 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFloatingPointNumber();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((float)1.5000000E+001);
        }
    }

    public class When_reading_a_long_floating_point_number_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private double _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 64, 111, 224, 0, 0, 0, 0, 0 });
        }

        protected override void When()
        {
            _readData = _reader.ReadLongFloatingPointNumber();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(2.5500000000000000E+002);
        }
    }

    public class When_reading_a_timestamp_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private DateTime _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 0, 0, 0, 0, 1, 0, 0, 5 });
        }

        protected override void When()
        {
            _readData = _reader.ReadTimestamp();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(new DateTime(1970, 7, 14, 4, 20, 21));
        }
    }

    public class When_reading_a_table_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private IDictionary<string, object> _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { 0, 0, 0, 8, 5, 65, 66, 67, 68, 69, (byte)'t', 1 });
        }

        protected override void When()
        {
            _readData = _reader.ReadTable();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData
                .Should().Count.One()
                .Should().Contain.Item(new KeyValuePair<string, object>("ABCDE", true));
        }
    }

    public class When_reading_boolean_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'t', 1 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(true);
        }
    }

    public class When_reading_short_short_integer_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'b', 5, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((sbyte)5);
        }
    }

    public class When_reading_a_byte_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'B', 5, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((byte)5);
        }
    }

    public class When_reading_a_short_integer_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'U', 1, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((short)262);
        }
    }

    public class When_reading_a_short_unsigned_integer_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'u', 1, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((ushort)262);
        }
    }

    public class When_reading_a_long_integer_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'I', 0, 0, 1, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(262);
        }
    }

    public class When_reading_a_long_unsigned_integer_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'i', 0, 0, 1, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((uint)262);
        }
    }

    public class When_reading_a_long_long_integer_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'L', 0, 0, 0, 0, 0, 0, 1, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((long)262);
        }
    }

    public class When_reading_a_long_long_unsigned_integer_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'l', 0, 0, 0, 0, 0, 0, 1, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((ulong)262);
        }
    }

    public class When_reading_a_floating_point_number_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'f', 65, 112, 0, 0 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((float)1.5000000E+001);
        }
    }

    public class When_reading_a_long_floating_point_number_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'d', 64, 111, 224, 0, 0, 0, 0, 0 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(2.5500000000000000E+002);
        }
    }

    public class When_reading_a_decimal_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'D', 2, 0, 0, 0, 4, 5 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal((decimal)0.04);
        }
    }

    public class When_reading_a_short_string_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'s', 5, 65, 66, 67, 68, 69, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal("ABCDE");
        }
    }

    public class When_reading_a_long_string_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'S', 0, 0, 0, 5, 65, 66, 67, 68, 69, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(new byte[] { 65, 66, 67, 68, 69 });
        }
    }

    public class When_reading_a_timestamp_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'T', 0, 0, 0, 0, 1, 0, 0, 5 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData.Should().Equal(new DateTime(1970, 7, 14, 4, 20, 21));
        }
    }

    public class When_reading_a_table_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private IDictionary<string, object> _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'F', 0, 0, 0, 8, 5, 65, 66, 67, 68, 69, (byte)'t', 1 });
        }

        protected override void When()
        {
            _readData = (IDictionary<string, object>)_reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData
                .Should().Count.One()
                .Should().Contain.Item(new KeyValuePair<string, object>("ABCDE", true));
        }
    }

    public class When_reading_an_array_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object[] _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'A', 0, 0, 0, 2, (byte)'t', 1 });
        }

        protected override void When()
        {
            _readData = (object[])_reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData
                .Should().Count.One()
                .Should().Contain.Item(true);
        }
    }

    public class When_reading_no_field_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new[] { (byte)'V' });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData
                .Should().Equal(null);
        }
    }

    public class When_reading_rabbitmq_qpid_array_field_value_via_amqp : XUnit2Specification
    {
        private AmqpReader _reader;
        private object _readData;

        protected override void Given()
        {
            _reader = new AmqpReader(new byte[] { (byte)'x', 0, 0, 0, 5, 0, 6, 1, 2, 3, 6 });
        }

        protected override void When()
        {
            _readData = _reader.ReadFieldValue();
        }

        [Fact]
        public void It_should_parse_correctly()
        {
            _readData
                .Should().Equal(new byte[] { 0, 6, 1, 2, 3 });
        }
    }
}