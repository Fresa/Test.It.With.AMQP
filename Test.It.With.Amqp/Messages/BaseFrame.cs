namespace Test.It.With.Amqp.Messages
{
    public abstract class BaseFrame<TMessage>
    {
        public abstract short Channel { get; }
        public abstract TMessage Message { get; }
    }
}