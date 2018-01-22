namespace Test.It.With.Amqp.Protocol._091
{
    internal class Amqp091ReaderFactory : IAmqpReaderFactory
    {
        public IAmqpReader Create(byte[] data)
        {
            return new Amqp091Reader(data);
        }
    }
}