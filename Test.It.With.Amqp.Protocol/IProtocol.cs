namespace Test.It.With.Amqp.Protocol
{
    public interface IProtocol
    {
        IVersion Version { get; }

        IMethod GetMethod(AmqpReader reader);

        IContentHeader GetContentHeader(AmqpReader reader);

        IContentBody GetContentBody(AmqpReader reader);
    }
}