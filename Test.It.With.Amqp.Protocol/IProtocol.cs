namespace Test.It.With.Amqp.Protocol
{
    public interface IProtocol
    {
        IVersion Version { get; }

        IMethod GetMethod(AmqpReader reader);
    }
}