using System;
using System.Collections.Generic;

namespace Test.It.With.Amqp.Protocol
{
    public interface IAmqpWriter : IDisposable
    {
        void WriteShortUnsignedInteger(ushort value);
        void WriteLongUnsignedInteger(uint value);
        void WriteLongLongUnsignedInteger(ulong value);
        void WriteShortInteger(short value);
        void WriteShortString(string value);
        void WriteCharacter(char value);
        void WriteLongString(byte[] value);
        void WriteLongInteger(int value);
        void WriteFloatingPointNumber(float value);
        void WriteLongFloatingPointNumber(double value);
        void WriteBytes(byte[] value);
        void WriteByte(byte value);
        void WriteTimestamp(DateTime value);
        void WriteTable(IDictionary<string, object> value);
        void WriteBoolean(bool value);
        void WriteShortShortInteger(sbyte value);
        void WriteLongLongInteger(long value);
        void WriteDecimal(decimal value);
        void WriteFieldValue(object value);
        void WritePropertyFlags(bool[] flags);
        void WriteBit(bool value);
    }
}