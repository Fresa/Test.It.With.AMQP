using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.It.With.Amqp.Protocol
{
    public class AmqpReader : IByteReader
    {
        private readonly byte[] _buffer;
        private int _position;
        private BitReader _bitReader;

        public AmqpReader(byte[] buffer)
        {
            _buffer = buffer;
            Length = _buffer.Length;

            _bitReader = new BitReader(this);
        }

        public int Length { get; }

        public ushort ReadShortUnsignedInteger()
        {
            _bitReader.Reset();
            return BitConverter.ToUInt16(ReadAsBigEndian(2), 0);
        }

        public uint ReadLongUnsignedInteger()
        {
            _bitReader.Reset();
            return BitConverter.ToUInt32(ReadAsBigEndian(4), 0);
        }

        public ulong ReadLongLongUnsignedInteger()
        {
            _bitReader.Reset();
            return BitConverter.ToUInt64(ReadAsBigEndian(8), 0);
        }

        public short ReadShortInteger()
        {
            _bitReader.Reset();
            return BitConverter.ToInt16(ReadAsBigEndian(2), 0);
        }

        public string ReadShortString()
        {
            _bitReader.Reset();
            int length = ReadByte();
            var bytes = ReadBytes(length);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public char ReadCharacter()
        {
            _bitReader.Reset();
            return BitConverter.ToChar(ReadAsLittleEndian(2), 0);
        }

        public byte[] ReadLongString()
        {
            _bitReader.Reset();
            var length = ReadLongUnsignedInteger();
            if (length > int.MaxValue)
            {
                throw new NotSupportedException($"Cannot handle long strings larger than {int.MaxValue}. Length detected: {length}.");
            }
            return ReadBytes((int)length);
        }

        public int ReadLongInteger()
        {
            _bitReader.Reset();
            return BitConverter.ToInt32(ReadAsBigEndian(4), 0);
        }

        public float ReadFloatingPointNumber()
        {
            _bitReader.Reset();
            return BitConverter.ToSingle(ReadAsBigEndian(4), 0);
        }

        public double ReadLongFloatingPointNumber()
        {
            _bitReader.Reset();
            return BitConverter.ToDouble(ReadAsBigEndian(8), 0);
        }

        public byte[] ReadBytes(int length)
        {
            _bitReader.Reset();
            return ReadAsLittleEndian(length);
        }

        public byte ReadByte()
        {
            _bitReader.Reset();
            return ReadAsLittleEndian(1).First();
        }

        public DateTime ReadTimestamp()
        {
            _bitReader.Reset();
            var posixTimestamp = ReadLongLongUnsignedInteger();
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(posixTimestamp);
        }

        public IDictionary<string, object> ReadTable()
        {
            _bitReader.Reset();
            IDictionary<string, object> table = new Dictionary<string, object>();
            var tableLength = ReadLongUnsignedInteger();

            var startPosition = _position;
            while (_position - startPosition < tableLength)
            {
                var name = ReadShortString();
                var value = ReadFieldValue();
                table[name] = value;
            }

            return table;
        }

        public bool ReadBoolean()
        {
            _bitReader.Reset();
            return BitConverter.ToBoolean(ReadAsLittleEndian(1), 0);
        }

        public sbyte ReadShortShortInteger()
        {
            _bitReader.Reset();
            return Convert.ToSByte(ReadByte());
        }

        public long ReadLongLongInteger()
        {
            _bitReader.Reset();
            return BitConverter.ToInt64(ReadAsBigEndian(8), 0);
        }

        public decimal ReadDecimal()
        {
            _bitReader.Reset();
            var scale = ReadByte();
            if (scale > 28)
            {
                throw new NotSupportedException("Decimals support up to a precision/scale of 28 digits.");
            }

            // NOTE! Contradiction in the AMQP 0.9.1 protocol description. 4.2.5.5 says "They are encoded as an octet representing the number of places followed by a long signed integer." But the 4.2.1. formal protocol grammar states a decimal as "scale long-uint". 
            // http://www.amqp.org/specification/0-9-1/amqp-org-download
            // We treat the value as signed integer (long-int).
            var value = ReadLongInteger();

            return new decimal(Math.Abs(value), 0, 0, value < 0, scale);
        }
        
        public bool ReadBit()
        {
            return _bitReader.Read();
        }

        public object ReadFieldValue()
        {
            _bitReader.Reset();
            var name = Convert.ToChar(ReadByte());

            switch (name)
            {
                case 't':
                    return ReadBoolean();
                case 'b':
                    return ReadShortShortInteger();
                case 'B':
                    return ReadByte();
                case 'U':
                    return ReadShortInteger();
                case 'u':
                    return ReadShortUnsignedInteger();
                case 'I':
                    return ReadLongInteger();
                case 'i':
                    return ReadLongUnsignedInteger();
                case 'L':
                    return ReadLongLongInteger();
                case 'l':
                    return ReadLongLongUnsignedInteger();
                case 'f':
                    return ReadFloatingPointNumber();
                case 'd':
                    return ReadLongFloatingPointNumber();
                case 'D':
                    return ReadDecimal();
                case 's':
                    return ReadShortString();
                case 'S':
                    return ReadLongString();
                case 'A':
                    return ReadArray();
                case 'T':
                    return ReadTimestamp();
                case 'F':
                    return ReadTable();
                case 'V':
                    return null;

                // todo: should be moved to an explicit RabbitMqReader
                // NOTE! RabbitMQ / Qpid special, https://www.rabbitmq.com/amqp-0-9-1-errata.html#section_3
                case 'x':
                    return new ByteArray(ReadLongString());

                default:
                    throw new InvalidOperationException($"Unknown field: {name}");
            }
        }

        public byte[] PeekBytes(int count)
        {
            var bytes = ReadBytes(count);
            _position -= count;
            return bytes;
        }

        public byte PeekByte()
        {
            var @byte = ReadByte();
            _position--;
            return @byte;
        }

        public bool[] ReadPropertyFlags()
        {
            _bitReader.Reset();
            var propertyFlagSetAvailable = true;
            var flags = new List<bool>();
            while (propertyFlagSetAvailable)
            {
                var value = ReadShortUnsignedInteger();
                for (var bit = 15; bit > 0; bit--)
                {
                    flags.Add((value & (1 << bit)) != 0);
                }
                propertyFlagSetAvailable = (value & (1 << 0)) != 0;
            }

            return flags.ToArray();
        }

        private object[] ReadArray()
        {
            var length = ReadLongInteger();
            var array = new List<object>();

            var startPosition = _position;
            while (_position - startPosition < length)
            {
                array.Add(ReadFieldValue());
            }

            return array.ToArray();
        }

        private byte[] ReadAsLittleEndian(int length)
        {
            var bytes = Read(length);
            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        private byte[] ReadAsBigEndian(int length)
        {
            var bytes = Read(length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        private byte[] Read(int length)
        {
            if (_buffer.Length < _position + length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Tried to read outside the buffer. Buffer length: {_buffer.Length}, Position: {_position}, Read request length: {length}.");
            }

            var bytes = new byte[length];
            Array.Copy(_buffer, _position, bytes, 0, length);
            _position += length;
            return bytes;
        }
    }
}