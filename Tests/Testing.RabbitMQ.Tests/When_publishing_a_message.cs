﻿using System;
using System.Text;
using RabbitMQ.Client;
using Should.Fluent;
using Testing.Framework;
using Testing.Framework.ApplicationBuilders;
using Testing.Framework.Fixtures;
using Testing.Framework.Specifications;
using Xunit;

namespace Testing.RabbitMQ.Tests
{
    public class When_publishing_a_message : XUnitWindowsServiceSpecification<WindowsServiceFixture<TestApplicationBuilder>>
    {
        private RabbitMqTestFramework.ClientEnvelope<TestMessage> _testMessagePublished;

        protected override void Given(IServiceContainer container)
        {
            var rabbitMqTestServer = new RabbitMqTestFramework(new NewtonsoftSerializer(Encoding.UTF8), new Lazy<IConnectionFactory>(() => new ConnectionFactory()
            {
                ContinuationTimeout = new TimeSpan(0,0,0,10),
                HandshakeContinuationTimeout = new TimeSpan(0, 0, 0, 10),
                RequestedConnectionTimeout = 1000,
                SocketReadTimeout = 1000,
                SocketWriteTimeout = 1000
            }));
            rabbitMqTestServer.OnPublish<TestMessage>(envelope => _testMessagePublished = envelope);

            container.Register(() => rabbitMqTestServer.ConnectionFactory);
        }

        protected override void When()
        {

        }

        [Fact]
        public void It_should_have_published_the_message()
        {
            _testMessagePublished.Should().Not.Be.Null();
        }
    }

    public class XUnitWindowsServiceSpecification<TFixture> : WindowsServiceSpecification<TFixture>, IClassFixture<TFixture> 
        where TFixture : class, IWindowsServiceFixture, new()
    {
        public XUnitWindowsServiceSpecification()
        {
            SetFixture(new TFixture());
        }
    }
}