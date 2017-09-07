using Test.It.With.RabbitMQ.MessageClient;

namespace Test.It.With.RabbitMQ.Protocol
{
    public interface IMethod
    {
        int ClassId { get; }
        int MethodId { get; }

        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }
}