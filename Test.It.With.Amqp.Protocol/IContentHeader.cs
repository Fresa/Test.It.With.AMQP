namespace Test.It.With.Amqp.Protocol
{
    public interface IContentHeader
    {
        int ClassId { get; }
        long BodySize { get; }

        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }
}