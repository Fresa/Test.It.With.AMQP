namespace Test.It.With.Amqp.Protocol
{
    public interface IContentBody
    {
        byte[] Payload { get; }
        
        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }
}