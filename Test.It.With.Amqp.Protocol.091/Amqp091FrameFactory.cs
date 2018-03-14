namespace Test.It.With.Amqp.Protocol._091
{
    internal class Amqp091FrameFactory : IFrameFactory
    {
        public IFrame Create(short channel, IMethod method)
        {
            return new Amqp091Frame(Constants.FrameMethod, channel, method);
        }

        public IFrame Create(short channel, IHeartbeat heartbeat)
        {
            return new Amqp091Frame(Constants.FrameHeartbeat, channel, heartbeat);
        }

        public IFrame Create(short channel, IContentHeader header)
        {
            return new Amqp091Frame(Constants.FrameHeader, channel, header);
        }

        public IFrame Create(short channel, IContentBody body)
        {
            return new Amqp091Frame(Constants.FrameBody, channel, body);
        }

        public IFrame Create(IAmqpReader reader)
        {
            return Amqp091Frame.ReadFrom(reader);
        }
    }
}