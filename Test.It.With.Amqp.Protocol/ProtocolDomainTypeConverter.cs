using System;
using System.Collections.Generic;

namespace Test.It.With.Amqp.Protocol
{
    public class ProtocolDomainTypeConverter
    {
        public Type Convert(string type)
        {
            switch (type.ToLower())
            {
                case "bit":
                    return Type<bool>();
                case "octet":
                    return Type<byte>();
                case "short":
                    return Type<short>();
                case "long":
                    return Type<int>();
                case "longlong":
                    return Type<long>();
                case "shortstr":
                    return Type<string>();
                case "longstr":
                    return Type<byte[]>();
                case "timestamp":
                    return Type<DateTime>();
                case "table":
                    return Type<IDictionary<string, object>>();
            }

            throw new NotSupportedException($"Unknown type '{type}'.");
        }

        private static Type Type<T>()
        {
            return typeof(T);
        }

        public string GetReaderMethod(string type)
        {
            switch (type.ToLower())
            {
                case "bit":
                    return "ReadBit";
                case "octet":
                    return "ReadByte";
                case "short":
                    return "ReadShortInteger";
                case "long":
                    return "ReadLongInteger";
                case "longlong":
                    return "ReadLongLongInteger";
                case "shortstr":
                    return "ReadShortString";
                case "longstr":
                    return "ReadLongString";
                case "timestamp":
                    return "ReadTimestamp";
                case "table":
                    return "ReadTable";
            }

            throw new NotSupportedException($"Unknown type '{type}'.");
        }

        public string GetWriterMethod(string type)
        {
            switch (type.ToLower())
            {
                case "bit":
                    return "WriteBit";
                case "octet":
                    return "WriteByte";
                case "short":
                    return "WriteShortInteger";
                case "long":
                    return "WriteLongInteger";
                case "longlong":
                    return "WriteLongLongInteger";
                case "shortstr":
                    return "WriteShortString";
                case "longstr":
                    return "WriteLongString";
                case "timestamp":
                    return "WriteTimestamp";
                case "table":
                    return "WriteTable";
            }

            throw new NotSupportedException($"Unknown type '{type}'.");
        }
    }
}