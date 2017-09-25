using System;
using System.IO;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.RabbitMQ.NetworkClient;
using Test.It.With.RabbitMQ.Protocol;

namespace Test.It.With.RabbitMQ.MessageClient
{
    internal class FrameClient : ITypedMessageClient<Frame, Frame>
    {
        public FrameClient(INetworkClient networkClient)
        {
            networkClient.BufferReceived += (sender, args) =>
            {
                var reader = new AmqpReader(args.Buffer);
                var frame = Frame.ReadFrom(reader);
                Received?.Invoke(this, frame);
            };
        }

        public event EventHandler<Frame> Received;
        public event EventHandler Disconnected;
        public void Send(Frame frame)
        {
            Frame.Write(frame);
        }
    }

    internal class MethodFrameClient : ITypedMessageClient<MethodFrame, Frame>
    {
        public MethodFrameClient(ITypedMessageClient<Frame, Frame> frameClient, IProtocol protocol)
        {
            frameClient.Received += (sender, args) =>
            {
                if (args.Type == Constants.FrameMethod)
                {
                    var reader = new AmqpReader(args.Payload);
                    var method = protocol.GetMethod(reader);

                    Received?.Invoke(this, new MethodFrame(args.Channel, method));
                }
            };
        }

        public event EventHandler<MethodFrame> Received;
        public event EventHandler Disconnected;
        public void Send(Frame frame)
        {
            Frame.Write(frame);
        }
    }

    internal class MethodFrameClient<TMethod> : ITypedMessageClient<MethodFrame<TMethod>, Frame> where TMethod : IMethod
    {
        public MethodFrameClient(ITypedMessageClient<MethodFrame, Frame> methodFrameClient)
        {
            methodFrameClient.Received += (sender, args) =>
            {
                if (args.Method.GetType() == typeof(TMethod))
                {
                    Received?.Invoke(this, new MethodFrame<TMethod>(args.Channel, (TMethod)args.Method));
                }
            };
        }

        public event EventHandler<MethodFrame<TMethod>> Received;
        public event EventHandler Disconnected;
        public void Send(Frame frame)
        {
            Frame.Write(frame);
        }
    }

    //internal class MethodClient : ITypedMessageClient<IMethod, IMethod>
    //{
    //    private readonly ITypedMessageClient<Frame, Frame> _networkClient;

    //    public MethodClient(IProtocol protocol, ITypedMessageClient<Frame, Frame> frameClient)
    //    {
    //        _networkClient = frameClient;
    //        frameClient.Received += (sender, args) =>
    //        {
    //            if (args.Type == Constants.FrameMethod)
    //            {
    //                var reader = new AmqpReader(args.Payload);
    //                var method = protocol.GetMethod(reader);
    //                Received?.Invoke(this, method);
    //            }
    //        };
    //    }

    //    public event EventHandler<IMethod> Received;
    //    public event EventHandler Disconnected;
    //    public void Send(IMethod frame)
    //    {
    //        _networkClient.Send(Frame.);
    //        using (var stream = new MemoryStream())
    //        {
    //            using (var writer = new AmqpWriter(stream))
    //            {
    //                frame.WriteTo(writer);

    //            }
    //        }
    //    }
    //}

    //internal class TypesMethodClient<TMethod> : ITypedMessageClient<TMethod, IMethod>
    //{
    //    public TypesMethodClient(INetworkClient networkClient, AmqpReader reader)
    //    {
    //        networkClient.BufferReceived += (sender, args) =>
    //        {
    //            args.
    //        }
    //    }

    //    public event EventHandler<TMethod> Received;
    //    public event EventHandler Disconnected;
    //    public void Send(IMethod frame)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    internal class TypedMessageClient<TMessageReceive> : BaseTypedMessageClient<TMessageReceive>, ITypedMessageClient<TMessageReceive>
    {
        public TypedMessageClient(IMessageClient messageClient, ISerializer serializer) : base(messageClient, serializer)
        {
            base.Received += Received;
            base.Disconnected += Disconnected;
        }

        public new event EventHandler<TMessageReceive> Received;
        public new event EventHandler Disconnected;

        public new void Send<TMessage>(TMessage message)
        {
            base.Send(message);
        }
    }

    internal class TypedMessageClient<TMessageReceive, TMessageSend> : BaseTypedMessageClient<TMessageReceive>, ITypedMessageClient<TMessageReceive, TMessageSend>
    {
        public TypedMessageClient(IMessageClient messageClient, ISerializer serializer) : base(messageClient, serializer)
        {
            base.Received += Received;
            base.Disconnected += Disconnected;
        }

        public new event EventHandler<TMessageReceive> Received;
        public new event EventHandler Disconnected;

        public void Send(TMessageSend frame)
        {
            base.Send(frame);
        }
    }
}