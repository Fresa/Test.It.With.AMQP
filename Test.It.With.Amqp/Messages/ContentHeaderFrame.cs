using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
{
    internal class ContentHeaderFrame : BaseFrame<IContentHeader>
    {
        public ContentHeaderFrame(short channel, IContentHeader contentHeader)
        {
            Channel = channel;
            Message = contentHeader;
        }

        public override short Channel { get; }
        public override IContentHeader Message { get; }
    }
}