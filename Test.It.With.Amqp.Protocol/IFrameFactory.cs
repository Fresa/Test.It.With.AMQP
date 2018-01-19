namespace Test.It.With.Amqp.Protocol
{
    internal interface IFrameFactory
    {
        IFrame Create(short channel, IMethod method);
        IFrame Create(short channel, IHeartbeat heartbeat);
        IFrame Create(short channel, IContentHeader header);
        IFrame Create(short channel, IContentBody body);
    }
}