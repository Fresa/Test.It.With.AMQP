using System.IO;

namespace Test.It.With.Amqp.Protocol
{
    public interface IAmqpWriterFactory
    {
        IAmqpWriter Create(Stream stream);
    }
}