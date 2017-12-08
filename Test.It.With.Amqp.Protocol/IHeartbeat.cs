namespace Test.It.With.Amqp.Protocol
{
    public interface IHeartbeat
    {
        bool SentOnValidChannel(int channel);
        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }
}