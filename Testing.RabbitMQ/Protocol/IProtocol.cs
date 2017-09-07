using Test.It.With.RabbitMQ.MessageClient;

namespace Test.It.With.RabbitMQ.Protocol
{
    public interface IProtocol
    {
        IVersion Version { get; }

        IMethod GetMethod(AmqpReader reader);
    }
}