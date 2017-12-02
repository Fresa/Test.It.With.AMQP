using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Expectations
{
    internal interface IExpectationStateMachine
    {
        bool ShouldPass(ProtocolHeader protocolHeader);

        bool ShouldPass<TMethod>(int channel, TMethod method)
            where TMethod : IClientMethod;

        bool ShouldPass<TMethod>(int channel, IContentHeader contentHeader, out TMethod method)
            where TMethod : IClientMethod, IContentMethod;

        bool ShouldPass<TMethod>(int channel, IContentBody contentBody, out TMethod method)
            where TMethod : IClientMethod, IContentMethod;
    }

    internal class Amqp091ExpectationStateMachine : IExpectationStateMachine
    {
        public Amqp091ExpectationStateMachine()
        {
            _expectationManager = new ExpectationBuilder()
                .WhenProtocolHeader().Then<Connection.StartOk>().Or<Connection.Close>()
                .When<Connection.CloseOk>().ThenProtocolHeader()
                .When<Connection.StartOk>().Then<Connection.SecureOk>().Or<Connection.Close>()
                .When<Connection.SecureOk>().Then<Connection.TuneOk>().Or<Connection.Close>()
                .When<Connection.TuneOk>().Then<Connection.Open>().Or<Connection.Close>()
                .When<Connection.Close>().ThenProtocolHeader()
                .When<Channel.Close>().Then<Channel.Open>()
                .When<Channel.CloseOk>().Then<Channel.Open>()
                .Manager;
        }
        // todo: Set agreed content body frame size

        private short _channelMax = short.MaxValue;
        private long _frameMax = Constants.FrameMinSize;

        private readonly ConcurrentDictionary<int, Expectation> _expectations = new ConcurrentDictionary<int, Expectation>(new Dictionary<int, Expectation>
        {
            { 0, new ProtocolHeaderExpectation() }
        });

        private readonly Dictionary<int, IContentMethod> _contentMethodStates = new Dictionary<int, IContentMethod>();

        private readonly ExpectingManager _expectationManager;

        public bool ShouldPass(ProtocolHeader protocolHeader)
        {
            var expectation = _expectations[0];
            
            if (expectation is ProtocolHeaderExpectation == false)
            {
                throw new UnexpectedFrameException(
                    $"Expected {expectation.Name}, got protocol header");
            }

            _expectations[0] = new MethodExpectation(_expectationManager.GetExpectingTypes<ProtocolHeader>());
            return true;
        }

        public bool ShouldPass<TMethod>(int channel, TMethod method)
            where TMethod : IClientMethod
        {
            if (method.SentOnValidChannel(channel) == false)
            {
                throw new ChannelErrorException($"{ method.GetType()} method is not valid on channel {channel}.");
            }

            if (channel > _channelMax)
            {
                throw new ChannelErrorException($"Channel {channel} not allowed. Maximum channel allowed is {_channelMax}.");
            }

            if (_expectations.TryGetValue(channel, out var expectation) == false)
            {
                if (method.GetType() != typeof(Channel.Open))
                {
                    throw new CommandInvalidException("Expected Channel.Open.");
                }

                expectation = new MethodExpectation(_expectationManager.GetExpectingTypes<TMethod>(method.GetType()));
                _expectations.TryAdd(channel, expectation);
            }

            switch (expectation)
            {
                case MethodExpectation methodExpectation:
                    if (methodExpectation.MethodResponses.Any())
                    {
                        if (methodExpectation.MethodResponses.Contains(method.GetType()) == false)
                        {
                            throw new UnexpectedFrameException($"Did not expect { method.GetType().FullName}. Expected: {string.Join(", ", methodExpectation.MethodResponses.Select(type => type.FullName))}.");
                        }
                    }

                    if (method is Connection.TuneOk tuneOk)
                    {
                        // todo: need to check against server proposal
                        _channelMax = tuneOk.ChannelMax.Value == 0 ? short.MaxValue : tuneOk.ChannelMax.Value;
                        // todo: need to check against server proposal
                        _frameMax = tuneOk.FrameMax.Value == 0 ? long.MaxValue : tuneOk.FrameMax.Value;
                    }

                    if (method is IContentMethod contentMethod)
                    {
                        _expectations[channel] = new ContentHeaderExpectation();
                        _contentMethodStates[channel] = contentMethod;
                        return false;
                    }

                    expectation = new MethodExpectation(_expectationManager.GetExpectingTypes<TMethod>(method.Responses()));
                    break;

                default:
                    throw new UnexpectedFrameException(
                        $"Expected method frame, got {expectation.GetType().Name} frame.");
            }

            _expectations[channel] = expectation;

            return true;
        }

        public bool ShouldPass<TMethod>(int channel, IContentHeader contentHeader, out TMethod method)
            where TMethod : IClientMethod, IContentMethod
        {
            if (channel == 0)
            {
                throw new ChannelErrorException("A content header cannot be sent on channel 0.");
            }

            if (_expectations.TryGetValue(channel, out var expectation) == false)
            {
                throw new UnexpectedFrameException("Channel has not been established. Expected Channel.Open.");
            }

            if (expectation is ContentHeaderExpectation == false)
            {
                throw new UnexpectedFrameException($"Expected {expectation.Name} frame, got content header frame.");
            }

            if (_contentMethodStates[channel].GetType() != typeof(TMethod))
            {
                method = default;
                return false;
            }

            _contentMethodStates[channel].SetContentHeader(contentHeader);

            if (contentHeader.BodySize > 0)
            {
                _expectations[channel] = new ContentBodyExpectation(contentHeader.BodySize);
                method = default;
                return false;
            }

            method = (TMethod)_contentMethodStates[channel];
            _contentMethodStates.Remove(channel);

            _expectations[channel] = new MethodExpectation(_expectationManager.GetExpectingTypes<TMethod>());
            return true;
        }

        public bool ShouldPass<TMethod>(int channel, IContentBody contentBody, out TMethod method)
            where TMethod : IClientMethod, IContentMethod
        {
            if (channel == 0)
            {
                throw new ChannelErrorException("A content body cannot be sent on channel 0.");
            }

            if (_expectations.TryGetValue(channel, out var expectation) == false)
            {
                throw new UnexpectedFrameException("Channel has not been established. Expected Channel.Open.");
            }

            if (expectation is ContentBodyExpectation == false)
            {
                throw new UnexpectedFrameException($"Expected content body frame, got {expectation.Name} frame.");
            }

            if (_contentMethodStates[channel].GetType() != typeof(TMethod))
            {
                method = default;
                return false;
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
                _expectations[channel] = new MethodExpectation(_expectationManager.GetExpectingTypes<TMethod>());
                method = (TMethod)_contentMethodStates[channel];
                _contentMethodStates.Remove(channel);
                return true;
            }

            _expectations[channel] = new ContentBodyExpectation(contentBodyExpectation.Size - size);
            method = default;
            return false;
        }
    }

    public static class ClientMethodExtensions
    {
        public static Type[] AcceptedRespondingMethods(IMethod method)
        {
            switch (method)
            {
                case Connection.StartOk _: return new[] { typeof(Connection.Secure) };
            }

            return Array.Empty<Type>();
        }

    }

    internal class ExpectingNext : ExpectingBase
    {
        private readonly ExpectationBuilder _builder;

        public ExpectingNext(ExpectationBuilder builder)
        {
            _builder = builder;
        }

        private readonly List<Type> _methods = new List<Type>();

        public ExpectingOr Then<TClient>() where TClient : IClientMethod
        {
            _methods.Add(typeof(TClient));
            return new ExpectingOr(_builder, _methods);
        }

        public ExpectingOr ThenProtocolHeader()
        {
            _methods.Add(typeof(ProtocolHeader));
            return new ExpectingOr(_builder, _methods);
        }

        public override Type[] Types => _methods.ToArray();
    }

    internal class ExpectingOr : ExpectingBase
    {
        private readonly ExpectationBuilder _builder;
        private readonly ExpectingNext _next;
        private List<Type> Methods { get; }

        public ExpectingOr(ExpectationBuilder builder, List<Type> methods)
        {
            _builder = builder;
            Methods = methods;
        }

        public ExpectingOr Or<TClient>() where TClient : IClientMethod
        {
            Methods.Add(typeof(TClient));
            return this;
        }

        public ExpectingNext When<TClient>() where TClient : IClientMethod
        {
            return _builder.When<TClient>();
        }

        public ExpectingManager Manager => new ExpectingManager(_builder);

        public override Type[] Types => Methods.ToArray();
    }

    internal abstract class ExpectingBase
    {
        public abstract Type[] Types { get; }
    }

    internal class ExpectationBuilder
    {
        private readonly Dictionary<Type, ExpectingBase> _expectations = new Dictionary<Type, ExpectingBase>();

        public IReadOnlyDictionary<Type, ExpectingBase> Expectations => _expectations;

        public ExpectingNext WhenProtocolHeader()
        {
            var expecting = new ExpectingNext(this);
            _expectations.Add(typeof(ProtocolHeader), expecting);
            return expecting;
        }

        public ExpectingNext When<TClient>() where TClient : IClientMethod
        {
            var expecting = new ExpectingNext(this);
            _expectations.Add(typeof(TClient), expecting);
            return expecting;
        }
    }

    public class Test
    {
        public Test()
        {
            var a = new ExpectationBuilder()
                .WhenProtocolHeader().Then<Connection.StartOk>().Or<Connection.Close>()
                .When<Connection.CloseOk>().ThenProtocolHeader()
                .When<Connection.StartOk>().Then<Connection.SecureOk>().Or<Connection.Close>()
                .When<Connection.SecureOk>().Then<Connection.TuneOk>().Or<Connection.Close>()
                .When<Connection.TuneOk>().Then<Connection.Open>().Or<Connection.Close>()
                .When<Connection.Close>().ThenProtocolHeader();

        }
    }

    internal class ExpectingManager
    {
        private readonly ExpectationBuilder _expectationBuilder;

        public ExpectingManager(ExpectationBuilder expectationBuilder)
        {
            _expectationBuilder = expectationBuilder;
        }

        public Type[] GetExpectingTypes<T>(params Type[] includeTypes)
        {
            var expectingTypes = new List<Type>(includeTypes);
            if (_expectationBuilder.Expectations.ContainsKey(typeof(T)))
            {
                expectingTypes.AddRange(_expectationBuilder.Expectations[typeof(T)].Types);
                return expectingTypes.ToArray();
            }

            return includeTypes;
        }
    }
}