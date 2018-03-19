using System;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.RabbitMQ
{
    internal class RabbitMQ091Reader : Amqp091Reader
    {
        public RabbitMQ091Reader(byte[] buffer) : base(buffer)
        {
        }

        public override object ReadFieldValue()
        {
            var name = Convert.ToChar((byte) PeekByte());
            switch (name)
            {
                case 'x':
                    ReadByte();
                    return new ByteArray(ReadLongString());
            }

            return base.ReadFieldValue();
        }
    }
}