namespace Testing.RabbitMQ.MessageClient
{
    internal class TypedMessageClientFactory
    {
        private readonly ISerializer _serializer;

        public TypedMessageClientFactory(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public ITypedMessageClient Create(IMessageClient messageClient)
        {
            return new TypedMessageClient(messageClient, _serializer);
        }

        public ITypedMessageClient<TMessage> Create<TMessage>(IMessageClient messageClient)
        {
            return new TypedMessageClient<TMessage>(messageClient, _serializer);
        }
    }


    
}