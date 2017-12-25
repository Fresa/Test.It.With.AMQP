namespace Test.It.With.Amqp.Protocol
{
    public interface IProtocolHeader
    {
        string Protocol { get; }
        IVersion Version { get; }
        bool IsValid { get; }
        void WriteTo(IAmqpWriter writer);
        void ReadFrom(IAmqpReader reader);
    }
}