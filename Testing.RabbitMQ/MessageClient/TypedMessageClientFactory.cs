namespace Test.It.With.RabbitMQ.MessageClient
{
    public class TypedMessageClientFactory
    {
        private readonly ISerializer _serializer;

        public TypedMessageClientFactory(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public ITypedMessageClient<TMessage> Create<TMessage>(IMessageClient messageClient)
        {
            return new TypedMessageClient<TMessage>(messageClient, _serializer);
        }

        public ITypedMessageClient<TReceiveMessage, TSendMessage> Create<TReceiveMessage, TSendMessage>(IMessageClient messageClient)
        {
            return new TypedMessageClient<TReceiveMessage, TSendMessage>(messageClient, _serializer);
        }
    }    
}