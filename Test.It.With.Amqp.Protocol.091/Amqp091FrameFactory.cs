namespace Test.It.With.Amqp.Protocol._091
{
    internal class Amqp091FrameFactory : IFrameFactory
    {
        private readonly IAmqpWriterFactory _amqpWriterFactory;

        public Amqp091FrameFactory(IAmqpWriterFactory amqpWriterFactory)
        {
            _amqpWriterFactory = amqpWriterFactory;
        }

        public IFrame Create(short channel, IMethod method)
        {
            return new Amqp091Frame(Constants.FrameMethod, channel, method, _amqpWriterFactory);
        }

        public IFrame Create(short channel, IHeartbeat heartbeat)
        {
            return new Amqp091Frame(Constants.FrameHeartbeat, channel, heartbeat, _amqpWriterFactory);
        }

        public IFrame Create(short channel, IContentHeader header)
        {
            return new Amqp091Frame(Constants.FrameHeader, channel, header, _amqpWriterFactory);
        }

        public IFrame Create(short channel, IContentBody body)
        {
            return new Amqp091Frame(Constants.FrameBody, channel, body, _amqpWriterFactory);
        }

        public IFrame Create(IAmqpReader reader)
        {
            return Amqp091Frame.ReadFrom(reader);
        }
    }
}