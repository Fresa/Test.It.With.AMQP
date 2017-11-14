using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Expectations
{
    internal class ExpectationStateMachine
    {
        // todo: Set agreed content body frame size

        private short _channelMax = short.MaxValue;
        private long _frameMax = Constants.FrameMinSize;

        private readonly ConcurrentDictionary<int, Expectation> _expectations = new ConcurrentDictionary<int, Expectation>();
        private Expectation _globalExpectation = new ProtocolHeaderExpectation();
        private readonly Dictionary<int, IContentMethod> _contentMethodStates = new Dictionary<int, IContentMethod>();

        public bool ShouldPass(ProtocolHeader protocolHeader)
        {
            if (_globalExpectation is ProtocolHeaderExpectation == false)
            {
                throw new UnexpectedFrameException(
                    $"Expected {_globalExpectation.Name}, got protocol header");
            }

            _globalExpectation = new MethodExpectation(new[] { typeof(Connection.StartOk) });

            return true;
        }

        public bool ShouldPass<TMethod>(int channel, TMethod method) where TMethod : IClientMethod
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

                expectation = new MethodExpectation(new[] { method.GetType() });
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

                    expectation = new MethodExpectation(method.Responses());
                    break;

                default:
                    throw new UnexpectedFrameException(
                        $"Expected method frame, got {expectation.GetType().Name} frame.");
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

            if (_expectations.TryGetValue(channel, out var expectation) == false)
            {
                throw new UnexpectedFrameException("Channel has not been established. Expected Channel.Open.");
            }

            if (expectation is ContentHeaderExpectation == false)
            {
                throw new UnexpectedFrameException($"Expected content header frame, got {expectation.Name} frame.");
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

            _expectations[channel] = new MethodExpectation();
            return true;
        }

        public bool ShouldPass<TMethod>(int channel, IContentBody contentBody, out TMethod method) where TMethod : IClientMethod
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
            method = default;
            return false;
        }
    }
}