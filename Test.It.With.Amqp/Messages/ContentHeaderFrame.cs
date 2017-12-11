using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
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

    public class ContentHeaderFrame<TContentHeader> : BaseFrame 
        where TContentHeader : class, IContentHeader
    {
        public ContentHeaderFrame(short channel, TContentHeader contentHeader)
        {
            Channel = channel;
            ContentHeader = contentHeader;
        }

        public override short Channel { get; }
        public TContentHeader ContentHeader { get; }
    }
}