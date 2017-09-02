using System;
using System.Data;

namespace Test.It.With.RabbitMQ.Protocol
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
                    return Type<string>();
                case "timestamp":
                    return Type<DateTime>();
                case "table":
                    return Type<DataTable>();
            }

            throw new NotSupportedException($"Unknown type '{type}'.");
        }

        private static Type Type<T>()
        {
            return typeof(T);
        }
    }
}