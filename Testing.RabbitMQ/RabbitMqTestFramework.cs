﻿using System;
using System.Linq;
using RabbitMQ.Client;
using Testing.RabbitMQ.MessageClient;
using Testing.RabbitMQ.NetworkClient;

namespace Testing.RabbitMQ
{
    public class RabbitMqTestFramework : IDisposable
    {
        private readonly ISerializer _serializer;
        private readonly INetworkClient _serverNetworkClient;
        private readonly CachedTestConnectionFactoryDecorator _connectionFactory;
        
        public RabbitMqTestFramework(ISerializer serializer, Lazy<IConnectionFactory> lazyConnectionFactory)
        {
            _serializer = serializer;

            var networkClientFactory = new InternalRoutedNetworkClientFactory();
            _serverNetworkClient = networkClientFactory.ServerNetworkClient;
            _connectionFactory = new CachedTestConnectionFactoryDecorator(
                new TestConnectionFactoryDecorator(
                    lazyConnectionFactory, networkClientFactory));
        }
        
        public IConnectionFactory ConnectionFactory => _connectionFactory;

        public void Consume<TMessage>(ServerEnvelope<TMessage> envelope)
        {
            var server = new TypedMessageClient(new MessageClient.MessageClient(_serverNetworkClient, _serializer), _serializer);
            server.Send(envelope);
        }

        public void OnPublish<TMessage>(Action<ClientEnvelope<TMessage>> messageProvider)
        {
            var server = new TypedMessageClient<ClientEnvelope<TMessage>>(new MessageClient.MessageClient(_serverNetworkClient, _serializer), _serializer);
            server.BufferReceived += (sender, envelope) => messageProvider(envelope);
        }

        public class ClientEnvelope<TMessage>
        {
            public ClientEnvelope(string exchange, string routingKey)
            {
                Exchange = exchange;
                RoutingKey = routingKey;
            }

            public string Exchange { get; }
            public string RoutingKey { get; }
            public TMessage Message { get; set; }
            public bool Mandatory { get; set; }
            public IBasicProperties Properties { get; set; }
        }

        public class ServerEnvelope<TMessage>
        {
            public ServerEnvelope(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string queue, string routingKey, IBasicProperties properties, TMessage message)
            {
                ConsumerTag = consumerTag;
                DeliveryTag = deliveryTag;
                Redelivered = redelivered;
                Exchange = exchange;
                RoutingKey = routingKey;
                Properties = properties;
                Message = message;
                Queue = queue;
            }

            public string ConsumerTag { get; }
            public ulong DeliveryTag { get; }
            public bool Redelivered { get; }
            public string Exchange { get; }
            public string RoutingKey { get; }
            public string Queue { get; }
            public IBasicProperties Properties { get; }
            public TMessage Message { get; }
        }

        public class ServerEnvelope
        {
            public ServerEnvelope(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string queue, string routingKey, IBasicProperties properties, byte[] message)
            {
                ConsumerTag = consumerTag;
                DeliveryTag = deliveryTag;
                Redelivered = redelivered;
                Exchange = exchange;
                RoutingKey = routingKey;
                Properties = properties;
                Message = message;
                Queue = queue;
            }

            public string ConsumerTag { get; }
            public ulong DeliveryTag { get; }
            public bool Redelivered { get; }
            public string Exchange { get; }
            public string RoutingKey { get; }
            public string Queue { get; }
            public IBasicProperties Properties { get; }
            public byte[] Message { get; }
        }

        public void Dispose()
        {
            _serverNetworkClient.Dispose();
        }
    }
}