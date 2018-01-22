using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageRouters
{
    internal class ContentHeaderFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<ContentHeaderFrame> _contentHeaderFrameHandler;
        private readonly IAmqpReaderFactory _readerFactory;

        public ContentHeaderFrameRouter(IHandle<IFrame> next, IProtocol protocol,
            IHandle<ContentHeaderFrame> contentHeaderFrameHandler, IAmqpReaderFactory readerFactory) : base(next)
        {
            _protocol = protocol;
            _contentHeaderFrameHandler = contentHeaderFrameHandler;
            _readerFactory = readerFactory;
        }

        public override void Handle(IFrame frame)
        {
            if (frame.IsHeader())
            {
                var reader = _readerFactory.Create(frame.Payload);
                var contentHeader = _protocol.GetContentHeader(reader);
                _contentHeaderFrameHandler.Handle(new ContentHeaderFrame( frame.Channel, contentHeader));
                return;
            }

            Next(frame);
        }
    }
}