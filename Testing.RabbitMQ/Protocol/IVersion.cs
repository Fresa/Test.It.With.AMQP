namespace Test.It.With.RabbitMQ.Protocol
{
    public interface IVersion
    {
        int Major { get; }
        int Minor { get; }
        int Revision { get; }
    }
}