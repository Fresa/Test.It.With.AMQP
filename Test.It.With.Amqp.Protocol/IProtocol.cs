namespace Test.It.With.Amqp.Protocol
{
    public interface IProtocol
    {
        IVersion Version { get; }
        IProtocolHeader GetProtocolHeader(AmqpReader reader);
        IMethod GetMethod(AmqpReader reader);
        IContentHeader GetContentHeader(AmqpReader reader);
        IContentBody GetContentBody(AmqpReader reader);
        IHeartbeat GetHeartbeat(AmqpReader reader);
    }
}