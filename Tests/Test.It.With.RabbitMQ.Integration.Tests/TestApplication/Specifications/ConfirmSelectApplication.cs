using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using SimpleInjector;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication.Specifications
{
    public class ConfirmSelectApplication : IApplication
    {
        private SimpleInjectorDependencyResolver _configurer;
        private RabbitMqLogEventListener _rabbitmqLogger;

        public void Configure(Action<SimpleInjectorDependencyResolver> reconfigurer)
        {
            _rabbitmqLogger = new RabbitMqLogEventListener();
            var container = new Container();
            container.RegisterSingleton<IConnectionFactory, ConnectionFactory>();
            container.RegisterSingleton<ISerializer>(() => new NewtonsoftSerializer(Encoding.UTF8));
            container.RegisterSingleton<IMessagePublisherFactory, RabbitMqMessagePublisherFactory>();
            container.RegisterSingleton<IRabbitMqPublisherSettings>(() => new DefaultRabbitMqPublisherSettings
            {
                EnablePublisherConfirms = true
            });

            _configurer = new SimpleInjectorDependencyResolver(container);
            reconfigurer(_configurer);
            _configurer.Verify();
        }

        public void Start(params string[] args)
        {
            var messagePublisherFactory = _configurer.Resolve<IMessagePublisherFactory>();

            Task.Run(() =>
            {
                var publishConfirmTimeOut = TimeSpan.FromSeconds(3);
                using (var messagePublisher = messagePublisherFactory.Create("myExchange0"))
                {
                    var confirmed = messagePublisher
                        .Publish("myMessage",
                            new TestMessage("Testing sending a message using RabbitMQ"))
                        .WaitForConfirm(publishConfirmTimeOut, out var timedOut);

                    if (confirmed == false)
                    {
                        throw new InvalidOperationException("Failed waiting for publish confirm.");
                    }

                    if (timedOut)
                    {
                        throw new TimeoutException($"Timed out waiting for confirms. Waited for {publishConfirmTimeOut.TotalSeconds}s.");
                    }

                    messagePublisher
                        .Publish("messageConfirmed",
                            new TestMessage("Test message has been confirmed"));
                }
            }).ContinueWith(task =>
            {
                OnUnhandledException?.Invoke(task.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Stop()
        {
            _configurer.Dispose();
            _rabbitmqLogger.Dispose();
        }

        public event Action<Exception> OnUnhandledException;
    }
}