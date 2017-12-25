namespace Test.It.With.Amqp.Protocol
{
    public interface IFrame
    {
        void WriteTo(IAmqpWriter writer);
        byte[] Payload { get; }
        short Channel { get; }
        bool IsMethod();
        bool IsBody();
        bool IsHeader();
        bool IsHeartbeat();
    }
}