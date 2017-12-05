namespace Test.It.With.Amqp.Protocol
{
    public interface IContentHeader
    {
        int ClassId { get; }
        long BodySize { get; }

        bool SentOnValidChannel(int channel);
        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }
}