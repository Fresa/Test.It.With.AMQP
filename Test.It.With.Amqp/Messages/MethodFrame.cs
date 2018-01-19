using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
{
    public class MethodFrame : BaseFrame<IMethod>
    {
        public MethodFrame(short channel, IMethod method)
        {
            Channel = channel;
            Message = method;
        }

        public override short Channel { get; }
        public override IMethod Message { get; }
    }

    public class MethodFrame<TMethod> : BaseFrame<TMethod> where TMethod : class, IMethod
    {
        public MethodFrame(short channel, TMethod method)
        {
            Channel = channel;
            Message = method;
        }

        public override short Channel { get; }
        public override TMethod Message { get; }
    }
}