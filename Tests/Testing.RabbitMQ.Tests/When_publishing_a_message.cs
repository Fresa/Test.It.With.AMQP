﻿using System;
using System.Collections.Concurrent;
using System.Text;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Xunit;
using Xunit.Abstractions;
using Connection = Test.It.With.Amqp.Connection;

namespace Test.It.With.RabbitMQ.Tests
{
    namespace Given_a_rabbitmq_client
    {
        public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
        {
            private readonly ConcurrentBag<MethodFrame<Exchange.Declare>> _exchangeDeclare = new ConcurrentBag<MethodFrame<Exchange.Declare>>();
            private readonly ConcurrentBag<MethodFrame<Basic.Publish>> _basicPublish = new ConcurrentBag<MethodFrame<Basic.Publish>>();
            
            public When_publishing_a_message(ITestOutputHelper output) : base(output)
            {
            }

            protected override void Given(IServiceContainer container)
            {
                var testServer = new AmqpTestFramework(ProtocolVersion.AMQP091);
                
                testServer.On((Func<ClientId, ProtocolHeaderFrame<ProtocolHeader>, Connection.Start>) ((clientId, handler) => new Connection.Start
                {
                    VersionMajor = new Octet((byte)handler.ProtocolHeader.Version.Major),
                    VersionMinor = new Octet((byte)handler.ProtocolHeader.Version.Minor),
                    Locales = new Longstr(Encoding.UTF8.GetBytes("en_US")),
                    Mechanisms = new Longstr(Encoding.UTF8.GetBytes("PLAIN")),
                }));
                testServer.On<Connection.StartOk>((client,frame) =>
                {
                    testServer.Send(client, new MethodFrame<Connection.Secure>(frame.Channel, new Connection.Secure
                    {
                        Challenge = new Longstr(Encoding.UTF8.GetBytes("challenge"))
                    }));
                });
                testServer.On<Connection.SecureOk>((clientId, frame) =>
                {
                    testServer.Send(clientId, new MethodFrame<Connection.Tune>(frame.Channel, new Connection.Tune
                    {
                        ChannelMax = new Short(0),
                        FrameMax = new Long(0),
                        Heartbeat = new Short(5)
                    }));
                });
                testServer.On<Connection.TuneOk>((__, _) => { });
                testServer.On<Connection.Open, Connection.OpenOk>((clientId, frame) => new Connection.OpenOk());
                testServer.On<Connection.Close, Connection.CloseOk>((clientId, frame) => new Connection.CloseOk());
                testServer.On<Channel.Open, Channel.OpenOk>((clientId, frame) => new Channel.OpenOk());
                testServer.On<Channel.Close>((clientId, frame) =>
                {
                    testServer.Send(clientId, new MethodFrame<Channel.CloseOk>(frame.Channel, new Channel.CloseOk()));

                    if (_exchangeDeclare.Count == 2)
                    {
                        ServiceController.Stop();
                    }
                });
                testServer.On<Exchange.Declare, Exchange.DeclareOk>((clientId, frame) =>
                {
                    _exchangeDeclare.Add(frame);
                    return new Exchange.DeclareOk();
                });
                testServer.On<Basic.Publish>((clientId, frame) =>
                {
                    _basicPublish.Add(frame);
                });
                testServer.On<Heartbeat>((__, _) => { });

                container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
            }

            [Fact]
            public void It_should_have_published_messages()
            {
                _basicPublish.Should().Count.Exactly(2);
            }
            
            [Fact]
            public void It_should_have_published_the_correct_message_type()
            {
                _basicPublish.Should().Contain.Any(frame => frame.Method.ContentHeader.Type.Equals(Shortstr.From(typeof(TestMessage).FullName)));
            }

            [Fact]
            public void It_should_have_published_the_correct_message()
            {
                _basicPublish.Should().Contain.Any(frame => frame.Method.ContentBody.Deserialize<TestMessage>().Message.Equals("Testing sending a message using RabbitMQ"));
            }

            [Fact]
            public void It_should_have_published_the_message_on_the_correct_exchange()
            {
                _basicPublish.Should().Contain.One(frame => frame.Method.Exchange.Equals(ExchangeName.From("myExchange0")));
                _basicPublish.Should().Contain.One(frame => frame.Method.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_exchanges()
            {
                _exchangeDeclare.Should().Count.Exactly(2);
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_name()
            {
                _exchangeDeclare.Should().Contain.One(frame => frame.Method.Exchange.Equals(ExchangeName.From("myExchange0")));
                _exchangeDeclare.Should().Contain.One(frame => frame.Method.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_type()
            {
                _exchangeDeclare.Should().Contain.Any(frame => frame.Method.Type.Equals(Shortstr.From("topic")));
            }
        }
    }
}
