using System;
using RabbitMQ.Client;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal class PublishResult
    {
        private readonly IModel _model;

        public PublishResult(IModel model, string correlationId)
        {
            _model = model;
            CorrelationId = correlationId;
        }

        public string CorrelationId { get; private set; }

        public bool WaitForConfirm(TimeSpan timeout, out bool timedOut)
        {
            return _model.WaitForConfirms(timeout, out timedOut);
        }
    }
}