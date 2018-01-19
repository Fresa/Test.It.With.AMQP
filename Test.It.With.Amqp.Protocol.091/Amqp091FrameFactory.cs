namespace Test.It.With.Amqp.Protocol._091
{
    public class Amqp091FrameFactory : IFrameFactory
    {
        public IFrame Create(short channel, IMethod method)
        {
            return new Amqp091MethodFrame(channel, method);
        }

        public IFrame Create(short channel, IHeartbeat heartbeat)
        {
            return new Amqp091HeartbeatFrame(channel, heartbeat);
        }

        public IFrame Create(short channel, IContentHeader header)
        {
            return new Amqp091ContentHeaderFrame(channel, header);
        }

        public IFrame Create(short channel, IContentBody body)
        {
            return new Amqp091ContentBodyFrame(channel, body);
        }
    }
}