using System.IO;

namespace Test.It.With.Amqp.Protocol
{
    internal interface IAmqpWriterFactory
    {
        IAmqpWriter Create(Stream stream);
    }
}