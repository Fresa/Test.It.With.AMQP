using System;
using System.Collections.Generic;
using System.Linq;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.RabbitMQ.MessageClient;
using Test.It.With.RabbitMQ.Messages;
using Test.It.With.RabbitMQ.NetworkClient;
using Test.It.With.RabbitMQ.Protocol;
using ChannelErrorException = Test.It.With.Amqp.ChannelErrorException;
using Constants = RabbitMQ.Client.Framing.Constants;
using Frame = Test.It.With.RabbitMQ.Protocol.Frame;
using UnexpectedFrameException = Test.It.With.Amqp.UnexpectedFrameException;

namespace Test.It.With.RabbitMQ
{
    public class AmqpTestFramework : IDisposable
    {
        private readonly FrameClient _frameClient;
        private readonly ITypedMessageClient<ProtocolHeader, Frame> _protocolHeaderClient;
        private readonly ITypedMessageClient<MethodFrame, Frame> _methodFrameClient;
        private readonly ITypedMessageClient<ContentHeaderFrame, Frame> _contentHeaderFrameClient;
        private readonly InternalRoutedNetworkClientFactory _networkClientFactory;
        private readonly ITypedMessageClient<ContentBodyFrame, Frame> _contentBodyFrameClient;

        private readonly List<Type> _methodsSubscribedOn = new List<Type>();

        private readonly StateMachine _stateMachine = new StateMachine();

        public AmqpTestFramework()
        {
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

        private void _subscribeOn<TMethod>()
        {
            if (_methodsSubscribedOn.Contains(typeof(TMethod)))
            {
                throw new InvalidOperationException("It is only allowed to subscribe on a specific method once.");
            }

            _methodsSubscribedOn.Add(typeof(TMethod));
        }

        public void Send<TMessage>(MethodFrame<TMessage> frame) where TMessage : IServerMethod
        {
            _frameClient.Send(new Frame(Constants.FrameMethod, frame.Channel, frame.Method));
        }

        public void On<TClientMethod>(Action<MethodFrame<TClientMethod>> messageHandler)
            where TClientMethod : IClientMethod
        {
            _subscribeOn<TClientMethod>();

            var server = new MethodFrameClient<TClientMethod>(_methodFrameClient);
            server.Received += (sender, frame) =>
            {
                if (_stateMachine.ShouldPass(frame.Channel, frame.Method))
                {
                    messageHandler(frame);
                }
            };

            _contentHeaderFrameClient.Received += (sender, frame) =>
            {
                if (_stateMachine.ShouldPass(frame.Channel, frame.ContentHeader, out TClientMethod method))
                {
                    messageHandler(new MethodFrame<TClientMethod>(frame.Channel, method));
                }
            };

            _contentBodyFrameClient.Received += (sender, frame) =>
            {
                if (_stateMachine.ShouldPass(frame.Channel, frame.ContentBody, out TClientMethod method))
                {
                    messageHandler(new MethodFrame<TClientMethod>(frame.Channel, method));
                }
            };
        }

        public void On<TClientMethod, TServerMethod>(Func<MethodFrame<TClientMethod>, TServerMethod> messageHandler)
            where TClientMethod : IClientMethod, IRespond<TServerMethod>
            where TServerMethod : IServerMethod
        {
            On<TClientMethod>(frame =>
            {
                var response = messageHandler(frame);
                Send(new MethodFrame<TServerMethod>(frame.Channel, response));
            });
        }

        public void OnProtocolHeader(Action<ProtocolHeader> messageHandler)
        {
            _subscribeOn<ProtocolHeader>();

            _protocolHeaderClient.Received += (sender, header) =>
            {
                if (_stateMachine.ShouldPass(header))
                {
                    messageHandler(header);
                }
            };
        }

        public void OnProtocolHeader(Func<ProtocolHeader, Connection.Start> messageHandler)
        {
            _subscribeOn<ProtocolHeader>();

            _protocolHeaderClient.Received += (sender, header) =>
            {
                if (_stateMachine.ShouldPass(header))
                {
                    var response = messageHandler(header);
                    _protocolHeaderClient.Send(new Frame(Constants.FrameMethod, 0, response));
                }
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

    internal class ContentFrame
    {
        public IMethod Method { get; set; }
    }

    internal class StateMachine
    {
        // todo: Set agreed content body frame size

        private short _channelMax = short.MaxValue;
        private long _frameMax = Constants.FrameMinSize;

        private readonly Dictionary<int, Expectation> _expectations = new Dictionary<int, Expectation>();
        private Expectation _globalExpectation = new ProtocolHeaderExpectation();

        private readonly Dictionary<int, IContentMethod> _contentMethodStates = new Dictionary<int, IContentMethod>();

        public bool ShouldPass(ProtocolHeader protocolHeader)
        {
            if (_globalExpectation.Type != ExpectationType.ProtocolHeader)
            {
                throw new UnexpectedFrameException(
                    $"Expected {_globalExpectation.Type.AsString()}, got protocol header");
            }

            _globalExpectation = new MethodExpectation(new[] { typeof(Connection.StartOk) });

            return true;
        }

        public bool ShouldPass<TMethod>(int channel, TMethod method) where TMethod : IClientMethod
        {
            if (method.SentOnValidChannel(channel) == false)
            {
                throw new ChannelErrorException($"{method.GetType()} method is not valid on channel {channel}.");
            }

            if (channel > _channelMax)
            {
                throw new ChannelErrorException($"Channel {channel} not allowed. Maximum channel allowed is {_channelMax}.");
            }

            if (_expectations.TryGetValue(channel, out Expectation expectation) == false)
            {
                if (method.GetType() != typeof(Channel.Open))
                {
                    throw new CommandInvalidException("Expected Channel.Open.");
                }

                expectation = new MethodExpectation(new[] { method.GetType() });
                _expectations.Add(channel, expectation);
            }

            switch (expectation.Type)
            {
                case ExpectationType.Method:
                    var methodExpectation = (MethodExpectation)expectation;

                    if (methodExpectation.MethodResponses.Any())
                    {
                        if (methodExpectation.MethodResponses.Contains(method.GetType()) == false)
                        {
                            throw new UnexpectedFrameException($"Did not expect {method.GetType().FullName}. Expected: {string.Join(", ", methodExpectation.MethodResponses.Select(type => type.FullName))}.");
                        }
                    }

                    var tuneOk = method as Connection.TuneOk;
                    if (tuneOk != null)
                    {
                        // todo: need to check against server proposal
                        _channelMax = tuneOk.ChannelMax.Value == 0 ? short.MaxValue : tuneOk.ChannelMax.Value;
                        // todo: need to check against server proposal
                        _frameMax = tuneOk.FrameMax.Value == 0 ? long.MaxValue : tuneOk.FrameMax.Value;
                    }

                    var contentMethod = method as IContentMethod;
                    if (contentMethod != null)
                    {
                        _expectations[channel] = new ContentHeaderExpectation();
                        _contentMethodStates[channel] = contentMethod;
                        return false;
                    }

                    expectation = new MethodExpectation(method.Responses());
                    break;

                default:
                    throw new UnexpectedFrameException(
                        $"Expected method frame, got {expectation.Type.AsString()} frame.");
            }

            _expectations[channel] = expectation;

            return true;
        }

        public bool ShouldPass<TMethod>(int channel, IContentHeader contentHeader, out TMethod method) where TMethod : IClientMethod
        {
            if (channel == 0)
            {
                throw new ChannelErrorException("A content header cannot be sent on channel 0.");
            }

            if (_expectations.TryGetValue(channel, out Expectation expectation) == false)
            {
                throw new UnexpectedFrameException("Channel has not been established. Expected Channel.Open.");
            }

            if (expectation.Type != ExpectationType.ContentHeader)
            {
                throw new UnexpectedFrameException($"Expected content header frame, got {expectation.Type.AsString()} frame.");
            }

            if (contentHeader.BodySize > 0)
            {
                _expectations[channel] = new ContentBodyExpectation(contentHeader.BodySize);
                _contentMethodStates[channel].SetContentHeader(contentHeader);
                method = default(TMethod);
                return false;
            }

            _expectations[channel] = new MethodExpectation();
            method = (TMethod)_contentMethodStates[channel];
            _contentMethodStates.Remove(channel);
            return true;
        }

        public bool ShouldPass<TMethod>(int channel, IContentBody contentBody, out TMethod method) where TMethod : IClientMethod
        {
            if (channel == 0)
            {
                throw new ChannelErrorException("A content body cannot be sent on channel 0.");
            }

            if (_expectations.TryGetValue(channel, out Expectation expectation) == false)
            {
                throw new UnexpectedFrameException("Channel has not been established. Expected Channel.Open.");
            }

            if (expectation.Type != ExpectationType.ContentBody)
            {
                throw new UnexpectedFrameException($"Expected content body frame, got {expectation.Type.AsString()} frame.");
            }

            var contentBodyExpectation = (ContentBodyExpectation)expectation;

            var size = contentBody.Payload.Length;
            if (size > contentBodyExpectation.Size)
            {
                throw new FrameErrorException($"Invalid content body frame size. Expected {contentBodyExpectation.Size}, got {size}.");
            }

            if (size + 1 > _frameMax)
            {
                throw new FrameErrorException($"Invalid content body frame size. Maximum frame size is {_frameMax}. Current frame size was {size + 1}.");
            }

            _contentMethodStates[channel].AddContentBody(contentBody);

            if (size == contentBodyExpectation.Size)
            {
                _expectations[channel] = new MethodExpectation();
                method = (TMethod)_contentMethodStates[channel];
                _contentMethodStates.Remove(channel);
                return true;
            }

            _expectations[channel] = new ContentBodyExpectation(contentBodyExpectation.Size - size);
            method = default(TMethod);
            return false;
        }
    }

    internal static class ExpectationTypeExtensions
    {
        public static string GetName(this ExpectationType type)
        {
            return Enum.GetName(typeof(ExpectationType), type);
        }

        public static string AsString(this ExpectationType type)
        {
            return type.GetName().SplitOnUpperCase().Join(" ").ToLower();
        }
    }
}