using System;

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

    public struct ClientId
    {
        public ClientId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }
    }
}