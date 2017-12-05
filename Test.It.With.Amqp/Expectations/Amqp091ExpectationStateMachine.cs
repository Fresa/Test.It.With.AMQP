using System.Collections.Generic;
using System.Linq;
using Test.It.With.Amqp.Expectations.MethodExpectationBuilders;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Expectations
{
    internal class Amqp091ExpectationStateMachine : IExpectationStateMachine
    {
        public Amqp091ExpectationStateMachine()
        {
            _expectedMethodManager = new MethodExpectationBuilder()
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

        private readonly ExpectationManager _expectationManager = new ExpectationManager();
        
        private readonly Dictionary<int, IContentMethod> _contentMethodStates = new Dictionary<int, IContentMethod>();

        private readonly ExpectedMethodManager _expectedMethodManager;

        public bool ShouldPass(ProtocolHeader protocolHeader)
        {
            _expectationManager.Get<ProtocolHeaderExpectation>(0);

            _expectationManager.Set(0, new MethodExpectation(_expectedMethodManager.GetExpectingMethodsFor<ProtocolHeader>()));
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

            var methodExpectation = _expectationManager.Get<MethodExpectation>(channel);

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
                _expectationManager.Set(channel, new ContentHeaderExpectation());
                _contentMethodStates[channel] = contentMethod;
                return false;
            }

            methodExpectation = new MethodExpectation(_expectedMethodManager.GetExpectingMethodsFor<TMethod>().Add(method.Responses()));

            _expectationManager.Set(channel, methodExpectation);

            return true;
        }

        public bool ShouldPass<TMethod>(int channel, IContentHeader contentHeader, out TMethod method)
            where TMethod : IClientMethod, IContentMethod
        {
            if (contentHeader.SentOnValidChannel(channel) == false)
            {
                throw new ChannelErrorException($"{ contentHeader.GetType()} cannot be sent on channel {channel}.");
            }

            _expectationManager.Get<ContentHeaderExpectation>(channel);
            
            if (_contentMethodStates[channel].GetType() != typeof(TMethod))
            {
                method = default;
                return false;
            }

            _contentMethodStates[channel].SetContentHeader(contentHeader);

            if (contentHeader.BodySize > 0)
            {
                _expectationManager.Set(channel, new ContentBodyExpectation(contentHeader.BodySize));
                method = default;
                return false;
            }

            method = (TMethod)_contentMethodStates[channel];
            _contentMethodStates.Remove(channel);

            _expectationManager.Set(channel, new MethodExpectation(_expectedMethodManager.GetExpectingMethodsFor<TMethod>()));
            return true;
        }

        public bool ShouldPass<TMethod>(int channel, IContentBody contentBody, out TMethod method)
            where TMethod : IClientMethod, IContentMethod
        {
            if (contentBody.SentOnValidChannel(channel) == false)
            {
                throw new ChannelErrorException($"{ contentBody.GetType()} cannot be sent on channel {channel}.");
            }
            
            var contentBodyExpectation = _expectationManager.Get<ContentBodyExpectation>(channel);
            
            if (_contentMethodStates[channel].GetType() != typeof(TMethod))
            {
                method = default;
                return false;
            }
            
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
                _expectationManager.Set(channel, new MethodExpectation(_expectedMethodManager.GetExpectingMethodsFor<TMethod>()));
                method = (TMethod)_contentMethodStates[channel];
                _contentMethodStates.Remove(channel);
                return true;
            }

            _expectationManager.Set(channel, new ContentBodyExpectation(contentBodyExpectation.Size - size));
            method = default;
            return false;
        }
    }

    internal static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] array, params T[] items)
        {
            var list = new List<T>(array);
            list.AddRange(items);
            return list.ToArray();
        }
    }
}