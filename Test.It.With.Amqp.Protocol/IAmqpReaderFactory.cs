namespace Test.It.With.Amqp.Protocol
{
    public interface IAmqpReaderFactory
    {
        IAmqpReader Create(byte[] data);
    }
}