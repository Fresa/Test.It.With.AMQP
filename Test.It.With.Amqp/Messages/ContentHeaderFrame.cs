using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
{
    public class ContentHeaderFrame : BaseFrame<IContentHeader>
    {
        public ContentHeaderFrame(short channel, IContentHeader contentHeader)
        {
            Channel = channel;
            Message = contentHeader;
        }

        public override short Channel { get; }
        public override IContentHeader Message { get; }
    }

    public class ContentHeaderFrame<TContentHeader> : BaseFrame <TContentHeader>
        where TContentHeader : class, IContentHeader
    {
        public ContentHeaderFrame(short channel, TContentHeader messageHeader)
        {
            Channel = channel;
            Message = messageHeader;
        }

        public override short Channel { get; }
        public override TContentHeader Message { get; }
    }
}