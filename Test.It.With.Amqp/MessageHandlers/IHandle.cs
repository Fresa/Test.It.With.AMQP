namespace Test.It.With.Amqp.MessageHandlers
{
    internal interface IHandle<in T>
    {
        void Handle(T message);
    }
}