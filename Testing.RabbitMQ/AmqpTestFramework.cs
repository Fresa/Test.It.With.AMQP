using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client.Impl;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.RabbitMQ.MessageClient;
using Test.It.With.RabbitMQ.Messages;
using Test.It.With.RabbitMQ.NetworkClient;
using Test.It.With.RabbitMQ.Protocol;
using Constants = RabbitMQ.Client.Framing.Constants;
using Frame = Test.It.With.RabbitMQ.Protocol.Frame;
using NotImplementedException = System.NotImplementedException;
using UnexpectedFrameException = Test.It.With.Amqp.UnexpectedFrameException;

namespace Test.It.With.RabbitMQ
{
    public class AmqpTestFramework : IDisposable
    {
        private readonly FrameClient _frameClient;
        private readonly ITypedMessageClient<ProtocolHeader, Frame> _protocolHeaderClient;
        private readonly ITypedMessageClient<MethodFrame, Frame> _methodFrameClient;
        private ITypedMessageClient<ContentHeaderFrame, Frame> _contentHeaderFrameClient;
        private readonly InternalRoutedNetworkClientFactory _networkClientFactory;
        private ITypedMessageClient<ContentBodyFrame, Frame> _contentBodyFrameClient;

        public AmqpTestFramework()
        {
            // todo: thread on each connection (servernetworkclient) so it's possible to block on waiting for content or sync response per channel. Maybe have thread per channel aswell?
            _networkClientFactory = new InternalRoutedNetworkClientFactory(out var serverNetworkClient);
            ConnectionFactory = _networkClientFactory;

            var protocol = new AmqProtocol();
            _protocolHeaderClient = _frameClient = new FrameClient(serverNetworkClient);
            var methodFrameClient = new MethodFrameClient(_frameClient, protocol);
            var contentHeaderFrameClientChain = new ContentHeaderFrameClient(methodFrameClient, protocol);
            _contentBodyFrameClient = new ContentBodyFrameClient(contentHeaderFrameClientChain, protocol);
            _methodFrameClient = methodFrameClient;
            _contentHeaderFrameClient = contentHeaderFrameClientChain;
        }

        public INetworkClientFactory ConnectionFactory { get; }

        public void Send<TMessage>(MethodFrame<TMessage> frame) where TMessage : IServerMethod
        {
            _frameClient.Send(new Frame(Constants.FrameMethod, frame.Channel, frame.Method));
        }

        public void On<TClientMethod>(Action<MethodFrame<TClientMethod>> messageHandler)
            where TClientMethod : IClientMethod
        {
            var server = new MethodFrameClient<TClientMethod>(_methodFrameClient);
            server.Received += (sender, frame) =>
            {
                messageHandler(frame);
            };
        }

        public void On<TClientMethod, TServerMethod>(Func<MethodFrame<TClientMethod>, TServerMethod> messageHandler)
            where TClientMethod : IClientMethod, IRespond<TServerMethod>
            where TServerMethod : IServerMethod
        {
            var server = new MethodFrameClient<TClientMethod>(_methodFrameClient);
            server.Received += (sender, frame) =>
            {
                var responseMethod = messageHandler(frame);
                server.Send(new Frame(Constants.FrameMethod, frame.Channel, responseMethod));
            };
        }

        public void OnProtocolHeader(Action<ProtocolHeader> messageHandler)
        {
            _protocolHeaderClient.Received += (sender, header) =>
            {
                messageHandler(header);
            };
        }

        public void OnProtocolHeader(Func<ProtocolHeader, Connection.Start> messageHandler)
        {
            _protocolHeaderClient.Received += (sender, header) =>
            {
                var response = messageHandler(header);
                _protocolHeaderClient.Send(new Frame(Constants.FrameMethod, 0, response));
            };
        }

        public void Dispose()
        {
            _networkClientFactory.Dispose();
        }
    }

    internal enum ExpectationType
    {
        Method,
        ContentHeader,
        ContentBody,
        ProtocolHeader
    }

    internal class MethodExpectation : Expectation
    {
        public MethodExpectation()
        {
            MethodResponses = Array.Empty<Type>();
        }

        public MethodExpectation(Type[] methods)
        {
            MethodResponses = methods;
        }

        public override ExpectationType Type { get; } = ExpectationType.Method;

        public Type[] MethodResponses { get; }
    }

    internal class ContentHeaderExpectation : Expectation
    {
        public override ExpectationType Type { get; } = ExpectationType.ContentHeader;
    }

    internal class ProtocolHeaderExpectation : Expectation
    {
        public override ExpectationType Type { get; } = ExpectationType.ProtocolHeader;
    }

    internal class ContentBodyExpectation : Expectation
    {
        public ContentBodyExpectation(long size)
        {
            Size = size;
        }

        public override ExpectationType Type { get; } = ExpectationType.ContentBody;

        public long Size { get; }
    }

    internal abstract class Expectation
    {
        public abstract ExpectationType Type { get; }
    }

    internal class StateMachine
    {
        private Dictionary<int, Expectation> _states = new Dictionary<int, Expectation>();
        private Expectation _globalExpectation = new ProtocolHeaderExpectation();

        // todo: Spara method state när det finns content och lägg till content header och appenda body när detta kommer

        // todo: utöka IMethod med IContentMethod och lägg till två metoder, AddContentHeader och AppendBody

        // todo: Lägg till en ny On method på ramverket och låt folk lyssna på IContentMethod

        public ProtocolHeader Pass(int channel, ProtocolHeader protocolHeader)
        {
            if (channel != 0)
            {
                throw new CommandInvalidException("Expected protocol header on channel 0.");
            }

            if (_globalExpectation.Type != ExpectationType.ProtocolHeader)
            {
                throw new UnexpectedFrameException(
                    $"Expected {Enum.GetName(typeof(ExpectationType), _globalExpectation)}, got protocol header");
            }

            _globalExpectation = new MethodExpectation(new[] { typeof(Connection.StartOk) });

            return protocolHeader;
        }

        public IMethod Pass(int channel, IMethod method)
        {
            if (method.SentOnValidChannel(channel) == false)
            {
                throw new CommandInvalidException($"{method.GetType()} method is not valid on channel {channel}.");
            }

            if (_states.TryGetValue(channel, out Expectation expectation) == false)
            {
                if (method.GetType() != typeof(Channel.Open))
                {
                    throw new CommandInvalidException("Expected Channel.Open.");
                }

                expectation = new MethodExpectation(new[] { method.GetType() });
                _states.Add(channel, expectation);
            }

            switch (expectation.Type)
            {
                case ExpectationType.Method:
                    if (method.HasContent)
                    {
                        expectation = new ContentHeaderExpectation();
                        break;
                    }

                    var methodExpectation = (MethodExpectation) expectation;

                    if (methodExpectation.MethodResponses.Any())
                    {
                        if (methodExpectation.MethodResponses.Contains(method.GetType()) == false)
                        {
                            throw new UnexpectedFrameException($"Did not expect {method.GetType().FullName}. Expected: {string.Join(", ", methodExpectation.MethodResponses.Select(type => type.FullName))}.");
                        }
                    }

                    expectation = new MethodExpectation( method.Responses());
                    break;

                default:
                    throw new UnexpectedFrameException(
                        $"Expected method frame, got {Enum.GetName(typeof(ExpectationType), expectation.Type).ToLower()} frame.");
            }

            _states[channel] = expectation;

            return method;
        }

        public IContentHeader Pass(int channel, IContentHeader contentHeader)
        {
            if (channel == 0)
            {
                throw new CommandInvalidException("A content header cannot be sent on channel 0.");
            }

            if (_states.TryGetValue(channel, out Expectation expectation) == false)
            {
                throw new UnexpectedFrameException("Channel has not been established. Expected Channel.Open.");
            }

            if (expectation.Type != ExpectationType.ContentHeader)
            {
                throw new UnexpectedFrameException($"Expected content header frame, got {Enum.GetName(typeof(ExpectationType), expectation.Type).ToLower()} frame.");
            }

            if (contentHeader.BodySize > 0)
            {
                expectation = new ContentBodyExpectation(contentHeader.BodySize);
            }
            else
            {
                expectation = new MethodExpectation();
            }

            _states[channel] = expectation;

            return contentHeader;
        }

        public IContentBody Pass(int channel, IContentBody contentBody)
        {
            throw new NotImplementedException();
        }
    }
}