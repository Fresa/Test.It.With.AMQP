using Test.It.With.Amqp.MessageClient;
using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageRouters
{
    internal class ContentHeaderFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<ContentHeaderFrame> _contentHeaderFrameHandler;

        public ContentHeaderFrameRouter(IHandle<Frame> next, IProtocol protocol,
            IHandle<ContentHeaderFrame> contentHeaderFrameHandler) : base(next)
        {
            _protocol = protocol;
            _contentHeaderFrameHandler = contentHeaderFrameHandler;
        }

        public override void Handle(Frame frame)
        {
            if (frame.Type == Constants.FrameHeader)
            {
                var reader = new AmqpReader(frame.Payload);
                var contentHeader = _protocol.GetContentHeader(reader);
                _contentHeaderFrameHandler.Handle(new ContentHeaderFrame(frame.Channel, contentHeader));
                return;
            }

            Next(frame);
        }
    }
}