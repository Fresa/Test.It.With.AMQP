using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageClient
{
    internal class FrameClient : ITypedMessageClient<Frame, Frame>, ITypedMessageClient<ProtocolHeader, Frame>
    {
        private readonly INetworkClient _networkClient;

        public FrameClient(INetworkClient networkClient)
        {
            _networkClient = networkClient;
            networkClient.BufferReceived += (sender, args) =>
            {
                var reader = new AmqpReader(args.Buffer);
                if (reader.PeekByte() == 'A')
                {
                    var header = ProtocolHeader.ReadFrom(reader);
                    ReceivedProtocolHeader?.Invoke(this, header);
                    return;
                }

                var frame = Frame.ReadFrom(reader);

                if (Received == null)
                {
                    throw new InvalidOperationException($"Missing deliver on {frame.GetType().FullName}.");
                }

                Received.Invoke(this, frame);
            };

            networkClient.Disconnected += Disconnected;
        }

        public event EventHandler<Frame> Received;

        private event EventHandler<ProtocolHeader> ReceivedProtocolHeader;
        event EventHandler<ProtocolHeader> ITypedMessageClient<ProtocolHeader, Frame>.Received
        {
            add => ReceivedProtocolHeader += value;
            remove => ReceivedProtocolHeader -= value;
        }

        public event EventHandler Disconnected;
        public void Send(Frame frame)
        {
            _networkClient.Send(frame);
        }
    }

    internal class FrameClient2
    {
        private readonly INetworkClient _networkClient;

        public FrameClient2(INetworkClient networkClient, IHandle<ProtocolHeader> protocolHeaderHandler, IHandle<Frame> frameHandler)
        {
            _networkClient = networkClient;
            networkClient.BufferReceived += (sender, args) =>
            {
                var reader = new AmqpReader(args.Buffer);
                if (reader.PeekByte() == 'A')
                {
                    var header = ProtocolHeader.ReadFrom(reader);
                    protocolHeaderHandler.Handle(header);
                    return;
                }

                var frame = Frame.ReadFrom(reader);

                frameHandler.Handle(frame);
            };

        }

        public void Send(Frame frame)
        {
            _networkClient.Send(frame);
        }
    }

    internal class ProtocolHeaderHandler : IHandle<ProtocolHeader>
    {
        public event Action<ProtocolHeader> OnReceive;

        public void Handle(ProtocolHeader protocolHeader)
        {
            if (OnReceive == null)
            {
                throw new InvalidOperationException($"There are no subscribers that can handle {typeof(ProtocolHeader).FullName}.");
            }

            OnReceive.Invoke(protocolHeader);
        }
    }

    internal abstract class FrameRouter : IHandle<Frame>
    {
        private readonly IHandle<Frame> _next;
        public abstract void Handle(Frame message);

        protected FrameRouter(IHandle<Frame> next)
        {
            _next = next;
        }

        protected void Next(Frame message)
        {
            if (_next == null)
            {
                throw new InvalidOperationException($"There is no router for frame type {message.Type}-");
            }

            _next.Handle(message);
        }
    }

    internal class MethodFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<MethodFrame> _methodFrameHandler;

        public MethodFrameRouter(IHandle<Frame> next, IProtocol protocol, IHandle<MethodFrame> methodFrameHandler) : base(next)
        {
            _protocol = protocol;
            _methodFrameHandler = methodFrameHandler;
        }

        public override void Handle(Frame frame)
        {
            if (frame.Type == Constants.FrameMethod)
            {
                var reader = new AmqpReader(frame.Payload);
                var method = _protocol.GetMethod(reader);
                _methodFrameHandler.Handle(new MethodFrame(frame.Channel, method));
                return;
            }

            Next(frame);
        }
    }

    internal class Unsubscriber
    {
        private readonly Action _unsubscribe;

        public Unsubscriber(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Unsubscribe()
        {
            _unsubscribe();
        }
    }

    internal class MethodFrameHandler : IHandle<MethodFrame>
    {
        private readonly ConcurrentDictionary<Type, Action<MethodFrame<IMethod>>> _methodSubscriptions = new ConcurrentDictionary<Type, Action<MethodFrame<IMethod>>>();

        public Unsubscriber Subscribe<TMethod>(Action<MethodFrame<TMethod>> subscription) where TMethod : IMethod
        {
            if (_methodSubscriptions.TryAdd(typeof(TMethod),
                    frame => subscription(new MethodFrame<TMethod>(frame.Channel, (TMethod) frame.Method))) == false)
            {
                throw new InvalidOperationException($"There is already a subscription on {typeof(TMethod).FullName}.");
            }

            return new Unsubscriber(() => _methodSubscriptions.TryRemove(typeof(TMethod), out _));
        }
        
        public void Handle(MethodFrame methodFrame)
        {
            if (_methodSubscriptions.TryGetValue(methodFrame.Method.GetType(), out var deliver) == false)
            {
                throw new InvalidOperationException($"There is no subscriptions on {methodFrame.Method.GetType().FullName}.");
            }
            
            deliver(new MethodFrame<IMethod>(methodFrame.Channel, methodFrame.Method));
        }
    }

    internal class ContentHeaderFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<ContentHeaderFrame> _contentHeaderFrameHandler;

        public ContentHeaderFrameRouter(IHandle<Frame> next, IProtocol protocol, IHandle<ContentHeaderFrame> contentHeaderFrameHandler) : base(next)
        {
            _protocol = protocol;
            _contentHeaderFrameHandler = contentHeaderFrameHandler;
        }

        public override void Handle(Frame frame)
        {
            if (frame.Type == Constants.FrameHeader)
            {
                var reader = new AmqpReader(frame.Payload);
                var contentHeader = _protocol.GetContentHeader(reader);
                _contentHeaderFrameHandler.Handle(new ContentHeaderFrame(frame.Channel, contentHeader));
                return;
            }

            Next(frame);
        }
    }

    internal class ContentBodyFrameRouter : FrameRouter
    {
        private readonly IProtocol _protocol;
        private readonly IHandle<ContentBodyFrame> _contentBodyFrameHandler;

        public ContentBodyFrameRouter(IHandle<Frame> next, IProtocol protocol, IHandle<ContentBodyFrame> contentBodyFrameHandler) : base(next)
        {
            _protocol = protocol;
            _contentBodyFrameHandler = contentBodyFrameHandler;
        }

        public override void Handle(Frame frame)
        {
            if (frame.Type == Constants.FrameBody)
            {
                var reader = new AmqpReader(frame.Payload);
                var contentBody = _protocol.GetContentBody(reader);
                _contentBodyFrameHandler.Handle(new ContentBodyFrame(frame.Channel, contentBody));
                return;
            }

            Next(frame);
        }
    }

    internal interface IHandle<in T>
    {
        void Handle(T message);
    }
}