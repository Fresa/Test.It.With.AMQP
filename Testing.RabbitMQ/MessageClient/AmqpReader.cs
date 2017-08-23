using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.It.With.RabbitMQ.MessageClient
{
    public class AmqpReader
    {
        private readonly byte[] _buffer;
        private int _position;

        public AmqpReader(byte[] buffer)
        {
            _buffer = buffer;
        }
        
        public ushort ReadShortUnsignedInteger()
        {
            return BitConverter.ToUInt16(ReadAsBigEndian(2), 0);
        }

        public uint ReadLongUnsignedInteger()
        {
            return BitConverter.ToUInt32(ReadAsBigEndian(4), 0);
        }

        public ulong ReadLongLongUnsignedInteger()
        {
            return BitConverter.ToUInt64(ReadAsBigEndian(8), 0);
        }

        public short ReadShortInteger()
        {
            return BitConverter.ToInt16(ReadAsBigEndian(2), 0);
        }

        public string ReadShortString()
        {
            int length = ReadByte();
            var bytes = ReadBytes(length);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public char ReadCharacter()
        {
            return BitConverter.ToChar(ReadAsLittleEndian(2), 0);
        }

        public byte[] ReadLongString()
        {
            var length = ReadLongUnsignedInteger();
            if (length > int.MaxValue)
            {
                throw new NotSupportedException($"Cannot handle long strings larger than {int.MaxValue}. Length detected: {length}.");
            }
            return ReadBytes((int)length);
        }

        public int ReadLongInteger()
        {
            return BitConverter.ToInt32(ReadAsBigEndian(4), 0);
        }

        public float ReadFloatingPointNumber()
        {
            return BitConverter.ToSingle(ReadAsBigEndian(4), 0);
        }

        public double ReadLongFloatingPointNumber()
        {
            return BitConverter.ToDouble(ReadAsBigEndian(8), 0);
        }

        public byte[] ReadBytes(int length)
        {
            return ReadAsLittleEndian(length);
        }

        public byte ReadByte()
        {
            return ReadAsLittleEndian(1).First();
        }

        public DateTime ReadTimestamp()
        {
            var posixTimestamp = ReadLongLongUnsignedInteger();
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(posixTimestamp);
        }

        public IDictionary<string, object> ReadTable()
        {
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
            return BitConverter.ToBoolean(ReadAsLittleEndian(1), 0);
        }

        public sbyte ReadShortShortInteger()
        {
            return Convert.ToSByte(ReadByte());
        }

        public long ReadLongLongInteger()
        {
            return BitConverter.ToInt64(ReadAsBigEndian(8), 0);
        }


        public decimal ReadDecimal()
        {
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

        public object ReadFieldValue()
        {
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

                // NOTE! RabbitMQ / Qpid special, https://www.rabbitmq.com/amqp-0-9-1-errata.html#section_3
                case 'x':
                    return new ByteArray(ReadLongString());

                default:
                    throw new InvalidOperationException($"Unknown field: {name}");
            }
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
                throw new ArgumentOutOfRangeException();
            }

            var bytes = new byte[length];
            Array.Copy(_buffer, _position, bytes, 0, length);
            _position += length;
            return bytes;
        }
    }
}