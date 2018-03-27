using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using SimpleInjector;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    public class MessageConsumingApplicationSpecification : IApplication
    {
        private SimpleInjectorDependencyResolver _configurer;
        private RabbitMqLogEventListener _rabbitmqLogger;
        private readonly ConcurrentBag<IMessageConsumer> _messageConsumers = new ConcurrentBag<IMessageConsumer>();

        public void Configure(Action<SimpleInjectorDependencyResolver> reconfigurer)
        {
            _rabbitmqLogger = new RabbitMqLogEventListener();
            var container = new Container();
            container.RegisterSingleton<IConnectionFactory, ConnectionFactory>();
            container.RegisterSingleton<ISerializer>(() => new NewtonsoftSerializer(Encoding.UTF8));
            container.RegisterSingleton<IMessageConsumerFactory, RabbitMqMessageConsumerFactory>();
            container.RegisterSingleton<IMessagePublisherFactory, RabbitMqMessagePublisherFactory>();

            _configurer = new SimpleInjectorDependencyResolver(container);
            reconfigurer(_configurer);
            _configurer.Verify();
        }

        public void Start(params string[] args)
        {
            var threads = int.Parse(args.First());
            var messageConsumerFactory = _configurer.Resolve<IMessageConsumerFactory>();
            var messagePublisherFactory = _configurer.Resolve<IMessagePublisherFactory>();

            Task.Run(() =>
            {
                Parallel.For(0, threads, i =>
                {
                    _messageConsumers.Add(messageConsumerFactory.Consume<TestMessage>(
                        "myExchange" + i % 2,
                        "queue" + i % 2, 
                        "routing" + i % 2,
                        message =>
                        {
                            using (var messagePublisher = messagePublisherFactory.Create("myExchange" + i % 2))
                            {
                                messagePublisher.Publish("myMessage",
                                    new TestMessage(i + ": " + message.Message));
                            }
                        }));
                });
            }).ContinueWith(task =>
            {
                OnUnhandledException?.Invoke(task.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Stop()
        {
            foreach (var messageConsumer in _messageConsumers)
            {
                messageConsumer.Dispose();
            }
            _configurer.Dispose();
            _rabbitmqLogger.Dispose();
        }

        public event Action<Exception> OnUnhandledException;
    }
}