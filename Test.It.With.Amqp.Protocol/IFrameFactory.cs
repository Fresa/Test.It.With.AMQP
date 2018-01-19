namespace Test.It.With.Amqp.Protocol
{
    internal interface IFrameFactory
    {
        IFrame Create(short channel, IMethod method);
    }
}