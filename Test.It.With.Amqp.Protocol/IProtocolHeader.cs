namespace Test.It.With.Amqp.Protocol
{
    public interface IProtocolHeader
    {
        string Protocol { get; }
        IVersion Version { get; }
        bool IsValid { get; }
        void WriteTo(AmqpWriter writer);
        void ReadFrom(AmqpReader reader);
    }
}