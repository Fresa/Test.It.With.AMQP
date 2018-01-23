namespace Test.It.With.Amqp.Protocol
{
    public interface IVersion
    {
        int Major { get; }
        int Minor { get; }
        int Revision { get; }
        void WriteTo(IAmqpWriter writer);
        void ReadFrom(IAmqpReader reader);
    }
}