namespace Test.It.With.Amqp.Protocol
{
    public interface IContentHeader : IMessage
    {
        int ClassId { get; }
        long BodySize { get; }
    }
}