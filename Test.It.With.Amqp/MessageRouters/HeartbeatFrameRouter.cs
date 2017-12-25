using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091; // todo: cannot reference explicit protocol

namespace Test.It.With.Amqp.MessageRouters
{
    internal class HeartbeatFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<HeartbeatFrame> _heartbeatFrameHandler;

        public HeartbeatFrameRouter(IHandle<Frame> next, IProtocol protocol, IHandle<HeartbeatFrame> heartbeatFrameHandler) :
            base(next)
        {
            _protocol = protocol;
            _heartbeatFrameHandler = heartbeatFrameHandler;
        }

        public override void Handle(Frame frame)
        {
            if (frame.Type == Constants.FrameHeartbeat)
            {
                var reader = new AmqpReader(frame.Payload);
                var heartbeat = _protocol.GetHeartbeat(reader);
                _heartbeatFrameHandler.Handle(new HeartbeatFrame(frame.Channel, heartbeat));
                return;
            }

            Next(frame);
        }
    }
}