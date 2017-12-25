using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091; // todo: cannot reference explicit protocol

namespace Test.It.With.Amqp.MessageRouters
{
    internal class ContentBodyFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<ContentBodyFrame> _contentBodyFrameHandler;

        public ContentBodyFrameRouter(IHandle<IFrame> next, IProtocol protocol,
            IHandle<ContentBodyFrame> contentBodyFrameHandler) : base(next)
        {
            _protocol = protocol;
            _contentBodyFrameHandler = contentBodyFrameHandler;
        }

        public override void Handle(IFrame frame)
        {
            if (frame.IsBody())
            {
                var reader = new Amqp091Reader(frame.Payload);
                var contentBody = _protocol.GetContentBody(reader);
                _contentBodyFrameHandler.Handle(new ContentBodyFrame((short)frame.Channel, contentBody));
                return;
            }

            Next(frame);
        }
    }
}