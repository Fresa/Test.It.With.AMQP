namespace Test.It.With.Amqp.Protocol
{
    public interface IMessage
    {
        bool SentOnValidChannel(int channel);
        void ReadFrom(IAmqpReader reader);
        void WriteTo(IAmqpWriter writer);
    }
}