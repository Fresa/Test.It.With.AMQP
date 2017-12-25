using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091; // todo: cannot reference explicit protocol

namespace Test.It.With.Amqp.MessageRouters
{
    internal class ContentHeaderFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<ContentHeaderFrame> _contentHeaderFrameHandler;

        public ContentHeaderFrameRouter(IHandle<IFrame> next, IProtocol protocol,
            IHandle<ContentHeaderFrame> contentHeaderFrameHandler) : base(next)
        {
            _protocol = protocol;
            _contentHeaderFrameHandler = contentHeaderFrameHandler;
        }

        public override void Handle(IFrame frame)
        {
            if (frame.IsHeader())
            {
                var reader = new Amqp091Reader(frame.Payload);
                var contentHeader = _protocol.GetContentHeader(reader);
                _contentHeaderFrameHandler.Handle(new ContentHeaderFrame( frame.Channel, contentHeader));
                return;
            }

            Next(frame);
        }
    }
}