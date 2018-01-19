using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
{
    public class ContentBodyFrame : BaseFrame<IContentBody>
    {
        public ContentBodyFrame(short channel, IContentBody contentBody)
        {
            Channel = channel;
            Message = contentBody;
        }

        public override short Channel { get; }
        public override IContentBody Message { get; }
    }
}