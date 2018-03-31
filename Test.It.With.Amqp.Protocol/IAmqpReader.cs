using System;
using System.Collections.Generic;

namespace Test.It.With.Amqp.Protocol
{
    public interface IAmqpReader
    {
        IAmqpReader Clone();
        void ThrowIfMoreData();
        int Length { get; }
        ushort ReadShortUnsignedInteger();
        uint ReadLongUnsignedInteger();
        ulong ReadLongLongUnsignedInteger();
        short ReadShortInteger();
        string ReadShortString();
        char ReadCharacter();
        byte[] ReadLongString();
        int ReadLongInteger();
        float ReadFloatingPointNumber();
        double ReadLongFloatingPointNumber();
        byte[] ReadBytes(int length);
        byte ReadByte();
        DateTime ReadTimestamp();
        IDictionary<string, object> ReadTable();
        bool ReadBoolean();
        sbyte ReadShortShortInteger();
        long ReadLongLongInteger();
        decimal ReadDecimal();
        bool ReadBit();
        object ReadFieldValue();
        byte[] PeekBytes(int count);
        byte PeekByte();
        bool[] ReadPropertyFlags();
    }
}