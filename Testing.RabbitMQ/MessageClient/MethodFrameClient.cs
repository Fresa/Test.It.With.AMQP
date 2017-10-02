using System;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.RabbitMQ.Messages;
using Test.It.With.RabbitMQ.Protocol;

namespace Test.It.With.RabbitMQ.MessageClient
{
    internal class MethodFrameClient : ITypedMessageClient<MethodFrame, Frame>
    {
        private readonly ITypedMessageClient<Frame, Frame> _frameClient;

        public MethodFrameClient(ITypedMessageClient<Frame, Frame> frameClient, IProtocol protocol)
        {
            _frameClient = frameClient;
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
            _frameClient.Send(frame);
        }
    }

    internal class MethodFrameClient<TMethod> : ITypedMessageClient<MethodFrame<TMethod>, Frame> where TMethod : IMethod
    {
        private readonly ITypedMessageClient<MethodFrame, Frame> _methodFrameClient;

        public MethodFrameClient(ITypedMessageClient<MethodFrame, Frame> methodFrameClient)
        {
            _methodFrameClient = methodFrameClient;
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
            _methodFrameClient.Send(frame);
        }
    }
}