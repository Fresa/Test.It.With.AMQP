namespace Testing.RabbitMQ.Tests
{
    public interface IMessageQueueApplication
    {
        void DeclareExchange(string exchange);
        void DeclareQueue(string queue);
        void BindQueueToExchange(string queue, string exchange, string routingkey);
        void Send<TMessage>(TMessage message);
    }
}