using System.IO;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.RabbitMQ
{
    internal class RabbitMq091Writer : Amqp091Writer
    {
        public RabbitMq091Writer(Stream buffer) : base(buffer)
        {
        }

        public override void WriteFieldValue(object value)
        {
            switch (value)
            {
                case ByteArray convertedValue:
                    WriteByte((byte) 'x');
                    WriteLongString(convertedValue.Bytes);
                    return;
            }

            base.WriteFieldValue(value);
        }
    }
}