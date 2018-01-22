using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageRouters
{
    internal class MethodFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<MethodFrame> _methodFrameHandler;
        private readonly IAmqpReaderFactory _readerFactory;

        public MethodFrameRouter(IHandle<IFrame> next, IProtocol protocol, IHandle<MethodFrame> methodFrameHandler, IAmqpReaderFactory readerFactory) :
            base(next)
        {
            _protocol = protocol;
            _methodFrameHandler = methodFrameHandler;
            _readerFactory = readerFactory;
        }

        public override void Handle(IFrame frame)
        {
            if (frame.IsMethod())
            {
                var reader = _readerFactory.Create(frame.Payload);
                var method = _protocol.GetMethod(reader);
                _methodFrameHandler.Handle(new MethodFrame(frame.Channel, method));
                return;
            }

            Next(frame);
        }
    }
}