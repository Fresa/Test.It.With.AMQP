namespace Test.It.With.Amqp.Protocol
{
    public interface IProtocol
    {
        IVersion Version { get; }
        IProtocolHeader GetProtocolHeader(IAmqpReader reader);
        IMethod GetMethod(IAmqpReader reader);
        IContentHeader GetContentHeader(IAmqpReader reader);
        IContentBody GetContentBody(IAmqpReader reader);
        IHeartbeat GetHeartbeat(IAmqpReader reader);
    }
}