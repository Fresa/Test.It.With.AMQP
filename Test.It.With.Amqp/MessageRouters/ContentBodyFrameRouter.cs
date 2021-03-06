using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageRouters
{
    internal class ContentBodyFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<ContentBodyFrame> _contentBodyFrameHandler;
        private readonly IAmqpReaderFactory _readerFactory;

        public ContentBodyFrameRouter(IHandle<IFrame> next, IProtocol protocol,
            IHandle<ContentBodyFrame> contentBodyFrameHandler, IAmqpReaderFactory readerFactory) : base(next)
        {
            _protocol = protocol;
            _contentBodyFrameHandler = contentBodyFrameHandler;
            _readerFactory = readerFactory;
        }

        public override void Handle(IFrame frame)
        {
            if (frame.IsBody())
            {
                var reader = _readerFactory.Create(frame.Payload);
                var contentBody = _protocol.GetContentBody(reader);
                _contentBodyFrameHandler.Handle(new ContentBodyFrame(frame.Channel, contentBody));
                return;
            }

            Next(frame);
        }
    }
}