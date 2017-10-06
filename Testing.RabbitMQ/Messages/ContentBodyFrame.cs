using Test.It.With.Amqp.Protocol;

namespace Test.It.With.RabbitMQ.Messages
{
    public class ContentBodyFrame : BaseFrame
    {
        public ContentBodyFrame(short channel, IContentBody contentBody)
        {
            Channel = channel;
            ContentBody = contentBody;
        }

        public override short Channel { get; }
        public IContentBody ContentBody { get; }
    }
}