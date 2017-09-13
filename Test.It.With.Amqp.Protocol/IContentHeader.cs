namespace Test.It.With.Amqp.Protocol
{
    public interface IContentHeader
    {
        int ClassId { get; }

        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }
}