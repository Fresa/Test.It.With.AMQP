using System.Collections.Generic;
using Log.It;
using Microsoft.Diagnostics.Tracing;
using RabbitMQ.Client.Logging;

namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    public sealed class RabbitMqLogEventListener : EventListener
    {
        private readonly ILogger _logger = LogFactory.Create<RabbitMqLogEventListener>();

        public RabbitMqLogEventListener()
        {
            EnableEvents(RabbitMqClientEventSource.Log, EventLevel.LogAlways, RabbitMqClientEventSource.Keywords.Log);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            foreach (var pl in eventData.Payload)
            {
                var dict = pl as IDictionary<string, object>;
                string message;
                if (dict != null)
                {
                    var rex = new RabbitMqExceptionDetail(dict);
                    message = rex.ToString();
                }
                else
                {
                    message = pl.ToString();
                }

                switch (eventData.Level)
                {
                    case EventLevel.Critical:
                        _logger.Fatal(message);
                        break;
                    case EventLevel.Error:
                        _logger.Error(message);
                        break;
                    case EventLevel.LogAlways:
                    case EventLevel.Informational:
                        _logger.Info(message);
                        break;
                    case EventLevel.Warning:
                        _logger.Warning(message);
                        break;
                    case EventLevel.Verbose:
                        _logger.Debug(message);
                        break;
                }
            }
        }

        public override void Dispose()
        {
            DisableEvents(RabbitMqClientEventSource.Log);
        }
    }
}