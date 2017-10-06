using Test.It.With.Amqp.Protocol;

namespace Test.It.With.RabbitMQ.Messages
{
    public class ContentHeaderFrame : BaseFrame
    {
        public ContentHeaderFrame(short channel, IContentHeader contentHeader)
        {
            Channel = channel;
            ContentHeader = contentHeader;
        }

        public override short Channel { get; }
        public IContentHeader ContentHeader { get; }
    }
}