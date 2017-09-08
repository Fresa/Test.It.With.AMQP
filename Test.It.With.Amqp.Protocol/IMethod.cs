namespace Test.It.With.Amqp.Protocol
{
    public interface IMethod
    {
        int ClassId { get; }
        int MethodId { get; }

        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }
}