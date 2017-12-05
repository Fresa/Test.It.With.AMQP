namespace Test.It.With.Amqp.Protocol
{
    public interface IContentBody
    {
        byte[] Payload { get; }

        bool SentOnValidChannel(int channel);
        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }
}