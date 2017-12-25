namespace Test.It.With.Amqp.Protocol
{
    public interface IContentBody
    {
        byte[] Payload { get; }

        bool SentOnValidChannel(int channel);
        void ReadFrom(IAmqpReader reader);
        void WriteTo(IAmqpWriter writer);
    }
}