using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageRouters
{
    internal class HeartbeatFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<HeartbeatFrame> _heartbeatFrameHandler;
        private readonly IAmqpReaderFactory _readerFactory;

        public HeartbeatFrameRouter(IHandle<IFrame> next, IProtocol protocol, IHandle<HeartbeatFrame> heartbeatFrameHandler, IAmqpReaderFactory readerFactory) :
            base(next)
        {
            _protocol = protocol;
            _heartbeatFrameHandler = heartbeatFrameHandler;
            _readerFactory = readerFactory;
        }

        public override void Handle(IFrame frame)
        {
            if (frame.IsHeartbeat())
            {
                var reader = _readerFactory.Create(frame.Payload);
                var heartbeat = _protocol.GetHeartbeat(reader);
                _heartbeatFrameHandler.Handle(new HeartbeatFrame(frame.Channel, heartbeat));
                return;
            }

            Next(frame);
        }
    }
}