using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Test.It.With.Amqp.Protocol._091
{
    public class Amqp091Writer : IByteWriter, IAmqpWriter
    {
        private readonly Stream _buffer;
        private readonly BitWriter _bitWriter;

        public Amqp091Writer(Stream buffer)
        {
            _buffer = buffer;
            _bitWriter = new BitWriter(this);
        }

        public void WriteShortUnsignedInteger(ushort value)
        {
            _bitWriter.Flush();
            WriteAsBigEndian(BitConverter.GetBytes(value));
        }

        public void WriteLongUnsignedInteger(uint value)
        {
            _bitWriter.Flush();
            WriteAsBigEndian(BitConverter.GetBytes(value));
        }

        public void WriteLongLongUnsignedInteger(ulong value)
        {
            _bitWriter.Flush();
            WriteAsBigEndian(BitConverter.GetBytes(value));
        }

        public void WriteShortInteger(short value)
        {
            _bitWriter.Flush();
            WriteAsBigEndian(BitConverter.GetBytes(value));
        }

        public void WriteShortString(string value)
        {
            _bitWriter.Flush();
            value = value ?? string.Empty;
            var bytes = Encoding.UTF8.GetBytes(value);
            if (bytes.Length > 255)
            {
                throw new SyntaxErrorException("Short string cannot be longer than 255 characters.");
            }

            WriteByte((byte)bytes.Length);
            WriteBytes(bytes);
        }

        public void WriteCharacter(char value)
        {
            _bitWriter.Flush();
            WriteAsLittleEndian(BitConverter.GetBytes(value));
        }

        public void WriteLongString(byte[] value)
        {
            _bitWriter.Flush();
            value = value ?? Array.Empty<byte>();
            WriteLongUnsignedInteger((uint)value.Length);
            WriteBytes(value);
        }

        public void WriteLongInteger(int value)
        {
            _bitWriter.Flush();
            WriteAsBigEndian(BitConverter.GetBytes(value));
        }

        public void WriteFloatingPointNumber(float value)
        {
            _bitWriter.Flush();
            WriteAsBigEndian(BitConverter.GetBytes(value));
        }

        public void WriteLongFloatingPointNumber(double value)
        {
            _bitWriter.Flush();
            WriteAsBigEndian(BitConverter.GetBytes(value));
        }

        public void WriteBytes(byte[] value)
        {
            _bitWriter.Flush();
            value = value ?? Array.Empty<byte>();
            WriteAsLittleEndian(value);
        }

        public void WriteByte(byte value)
        {
            _bitWriter.Flush();
            WriteAsLittleEndian(new[] { value });
        }

        public void WriteTimestamp(DateTime value)
        {
            _bitWriter.Flush();
            var seconds = value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            WriteLongLongUnsignedInteger((ulong)seconds);
        }

        public void WriteTable(IDictionary<string, object> value)
        {
            _bitWriter.Flush();
            if (value == null || value.Count == 0)
            {
                WriteLongUnsignedInteger(0);
                return;
            }

            var startPosition = _buffer.Position;
            WriteLongUnsignedInteger(0);
            var startCountingLengthPosition = _buffer.Position;

            foreach (var entry in value)
            {
                WriteShortString(entry.Key);
                WriteFieldValue(entry.Value);
            }

            var currentPosition = _buffer.Position;
            _buffer.Seek(startPosition, SeekOrigin.Begin);
            WriteLongUnsignedInteger((uint)(currentPosition - startCountingLengthPosition));
            _buffer.Seek(currentPosition, SeekOrigin.Begin);
        }

        private void WriteTable(ICollection value)
        {
            if (value == null || value.Count == 0)
            {
                WriteLongUnsignedInteger(0);
                return;
            }

            var startPosition = _buffer.Position;
            WriteLongUnsignedInteger(0);
            var startCountingLengthPosition = _buffer.Position;

            foreach (DictionaryEntry entry in value)
            {
                WriteShortString(entry.Key.ToString());
                WriteFieldValue(entry.Value);
            }

            var currentPosition = _buffer.Position;
            _buffer.Seek(startPosition, SeekOrigin.Begin);
            WriteLongUnsignedInteger((uint)(currentPosition - startCountingLengthPosition));
            _buffer.Seek(currentPosition, SeekOrigin.Begin);
        }

        public void WriteBoolean(bool value)
        {
            _bitWriter.Flush();
            WriteAsLittleEndian(BitConverter.GetBytes(value));
        }

        public void WriteShortShortInteger(sbyte value)
        {
            _bitWriter.Flush();
            WriteByte((byte)value);
        }

        public void WriteLongLongInteger(long value)
        {
            _bitWriter.Flush();
            WriteAsBigEndian(BitConverter.GetBytes(value));
        }

        public void WriteDecimal(decimal value)
        {
            _bitWriter.Flush();
            var scale = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];

            for (var i = 0; i < scale; i++)
            {
                value *= 10;
            }

            if (value > int.MaxValue || value < int.MinValue)
            {
                throw new SyntaxErrorException($"{value} is out of AMQP bounds.");
            }

            WriteByte(scale);
            // NOTE! Contradiction in the AMQP 0.9.1 protocol description. 4.2.5.5 says "They are encoded as an octet representing the number of places followed by a long signed integer." But the 4.2.1. formal protocol grammar states a decimal as "scale long-uint". 
            // http://www.amqp.org/specification/0-9-1/amqp-org-download
            // We treat the value as signed integer (long-int).
            WriteLongInteger((int)value);
        }

        public void WriteFieldValue(object value)
        {
            _bitWriter.Flush();

            switch (value)
            {
                case bool convertedValue:
                    WriteByte((byte)'t');
                    WriteBoolean(convertedValue);
                    return;
                case sbyte convertedValue:
                    WriteByte((byte)'b');
                    WriteShortShortInteger(convertedValue);
                    return;
                case byte convertedValue:
                    WriteByte((byte)'B');
                    WriteByte(convertedValue);
                    return;
                case short convertedValue:
                    WriteByte((byte)'U');
                    WriteShortInteger(convertedValue);
                    return;
                case ushort convertedValue:
                    WriteByte((byte)'u');
                    WriteShortUnsignedInteger(convertedValue);
                    return;
                case int convertedValue:
                    WriteByte((byte)'I');
                    WriteLongInteger(convertedValue);
                    return;
                case uint convertedValue:
                    WriteByte((byte)'i');
                    WriteLongUnsignedInteger(convertedValue);
                    return;
                case long convertedValue:
                    WriteByte((byte)'L');
                    WriteLongLongInteger(convertedValue);
                    return;
                case ulong convertedValue:
                    WriteByte((byte)'l');
                    WriteLongLongUnsignedInteger(convertedValue);
                    return;
                case float convertedValue:
                    WriteByte((byte)'f');
                    WriteFloatingPointNumber(convertedValue);
                    return;
                case double convertedValue:
                    WriteByte((byte)'d');
                    WriteLongFloatingPointNumber(convertedValue);
                    return;
                case decimal convertedValue:
                    WriteByte((byte)'D');
                    WriteDecimal(convertedValue);
                    return;
                case string convertedValue:
                    WriteByte((byte)'s');
                    WriteShortString(convertedValue);
                    return;
                case byte[] convertedValue:
                    WriteByte((byte)'S');
                    WriteLongString(convertedValue);
                    return;
                case IList convertedValue:
                    WriteByte((byte)'A');
                    WriteArray(convertedValue);
                    return;
                case DateTime convertedValue:
                    WriteByte((byte)'T');
                    WriteTimestamp(convertedValue);
                    return;
                case IDictionary<string, object> convertedValue:
                    WriteByte((byte)'F');
                    WriteTable(convertedValue);
                    return;
                case IDictionary convertedValue:
                    WriteByte((byte)'F');
                    WriteTable(convertedValue);
                    return;
                case null:
                    WriteByte((byte)'V');
                    return;

                // todo: should be extracted to a rabbitmq / qpit specific amqp writer
                // NOTE! RabbitMQ / Qpid special, https://www.rabbitmq.com/amqp-0-9-1-errata.html#section_3
                case ByteArray convertedValue:
                    WriteByte((byte)'x');
                    WriteLongString(convertedValue.Bytes);
                    return;
                default:
                    throw new SyntaxErrorException($"Unknown field value type: {value.GetType()}");
            }
        }

        public void WritePropertyFlags(bool[] flags)
        {
            _bitWriter.Flush();
            flags = flags ?? Array.Empty<bool>();
            ushort value = 0;
            for (var i = 0; i < flags.Length; i++)
            {
                if (i > 0 && i % 15 == 0)
                {
                    value = (ushort)(value | 1);
                    WriteShortUnsignedInteger(value);
                    value = 0;
                }

                var bit = 15 - i % 15;

                if (flags[i])
                {
                    value = (ushort)(value | (1 << bit));
                }
            }

            WriteShortUnsignedInteger(value);
        }

        public void WriteBit(bool value)
        {
            _bitWriter.Write(value);
        }

        private void WriteArray(ICollection value)
        {
            var startPosition = _buffer.Position;
            WriteLongInteger(0);

            if (value == null || value.Count == 0)
            {
                return;
            }

            var startCountingLengthPosition = _buffer.Position;

            foreach (var entry in value)
            {
                WriteFieldValue(entry);
            }

            var currentPosition = _buffer.Position;
            _buffer.Seek(startPosition, SeekOrigin.Begin);
            WriteLongInteger((int)(currentPosition - startCountingLengthPosition));
            _buffer.Seek(currentPosition, SeekOrigin.Begin);
        }

        private void WriteAsLittleEndian(byte[] value)
        {
            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(value);
            }
            Write(value);
        }

        private void WriteAsBigEndian(byte[] value)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(value);
            }
            Write(value);
        }

        private void Write(byte[] value)
        {
            _buffer.Write(value, 0, value.Length);
        }

        public void Dispose()
        {
            _bitWriter.Dispose();
            _buffer.Flush();
        }
    }
}