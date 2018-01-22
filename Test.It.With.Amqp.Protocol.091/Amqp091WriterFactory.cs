using System.IO;

namespace Test.It.With.Amqp.Protocol._091
{
    internal class Amqp091WriterFactory : IAmqpWriterFactory
    {
        public IAmqpWriter Create(Stream stream)
        {
            return new Amqp091Writer(stream);
        }
    }
}