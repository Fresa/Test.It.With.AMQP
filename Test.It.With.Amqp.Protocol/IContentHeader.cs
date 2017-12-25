namespace Test.It.With.Amqp.Protocol
{
    public interface IContentHeader
    {
        int ClassId { get; }
        long BodySize { get; }

        bool SentOnValidChannel(int channel);
        void ReadFrom(IAmqpReader reader);
        void WriteTo(IAmqpWriter writer);
    }
}