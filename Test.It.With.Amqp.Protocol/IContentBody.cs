namespace Test.It.With.Amqp.Protocol
{
    public interface IContentBody : IMessage
    {
        byte[] Payload { get; }
    }
}