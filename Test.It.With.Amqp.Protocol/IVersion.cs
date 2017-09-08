namespace Test.It.With.Amqp.Protocol
{
    public interface IVersion
    {
        int Major { get; }
        int Minor { get; }
        int Revision { get; }
    }
}