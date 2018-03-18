namespace Test.It.With.Amqp.MessageClient
{
    internal interface ISender<in TMessage>
    {
        void Send(TMessage message);
    }
}