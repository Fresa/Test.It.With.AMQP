namespace Test.It.With.Amqp.Messages
{
    public class MethodFrame : BaseFrame
    {
        public MethodFrame(short channel, Amqp.Protocol.IMethod method)
        {
            Channel = channel;
            Method = method;
        }

        public override short Channel { get; }
        public Amqp.Protocol.IMethod Method { get; }
    }

    public class MethodFrame<TMethod> : BaseFrame where TMethod : class, Amqp.Protocol.IMethod
    {
        public MethodFrame(short channel, TMethod method)
        {
            Channel = channel;
            Method = method;
        }

        public override short Channel { get; }
        public TMethod Method { get; }
    }

    public class MethodFrame<TMethod, TContentHeader> : BaseFrame 
        where TMethod : Amqp.Protocol.IMethod, Protocol.IContentMethod
        where TContentHeader : Amqp.Protocol.IContentHeader
    {
        public MethodFrame(short channel, TMethod method, TContentHeader contentHeader, byte[] body)
        {
            Channel = channel;
            Method = method;
            ContentHeader = contentHeader;
            Body = body;
        }

        public override short Channel { get; }
        public TMethod Method { get; }
        public TContentHeader ContentHeader { get; }
        public byte[] Body { get; }
    }
}