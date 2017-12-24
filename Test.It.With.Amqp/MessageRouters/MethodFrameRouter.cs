using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp091; // todo: cannot reference explicit protocol

namespace Test.It.With.Amqp.MessageRouters
{
    internal class MethodFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<MethodFrame> _methodFrameHandler;

        public MethodFrameRouter(IHandle<Frame> next, IProtocol protocol, IHandle<MethodFrame> methodFrameHandler) :
            base(next)
        {
            _protocol = protocol;
            _methodFrameHandler = methodFrameHandler;
        }

        public override void Handle(Frame frame)
        {
            if (frame.Type == Constants.FrameMethod)
            {
                var reader = new AmqpReader(frame.Payload);
                var method = _protocol.GetMethod(reader);
                _methodFrameHandler.Handle(new MethodFrame(frame.Channel, method));
                return;
            }

            Next(frame);
        }
    }
}