using System;
using System.Collections.Generic;
using Test.It.With.Amqp.MessageHandlers;
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

        public FrameClient2(INetworkClient networkClient, IHandle<ProtocolHeader> protocolHeaderHandler,
            IHandle<Frame> frameHandler)
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
                reader.ThrowIfMoreData();

                frameHandler.Handle(frame);
            };
        }

        public void Send(Frame frame)
        {
            _networkClient.Send(frame);
        }
    }
}