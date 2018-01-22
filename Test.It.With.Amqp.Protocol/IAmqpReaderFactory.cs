namespace Test.It.With.Amqp.Protocol
{
    internal interface IAmqpReaderFactory
    {
        IAmqpReader Create(byte[] data);
    }
}