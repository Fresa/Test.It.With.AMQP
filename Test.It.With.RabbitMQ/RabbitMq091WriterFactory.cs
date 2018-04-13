using System.IO;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.RabbitMQ._091
{
    internal class RabbitMq091WriterFactory : IAmqpWriterFactory
    {
        public IAmqpWriter Create(Stream stream)
        {
            return new RabbitMq091Writer(stream);
        }
    }
}