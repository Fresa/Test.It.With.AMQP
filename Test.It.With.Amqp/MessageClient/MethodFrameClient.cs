using System;
using System.Linq;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageClient
{
    internal class MethodFrameClient : ITypedMessageClient<MethodFrame, Frame>, IChainableTypedMessageClient<Frame, Frame>
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

                    if (Received == null)
                    {
                        throw new InvalidOperationException($"Missing subscription on {method.GetType().FullName}.");
                    }

                    Received.Invoke(this, new MethodFrame(args.Channel, method));
                }
                else
                {
                    if (Next == null)
                    {
                        throw new InvalidOperationException($"Missing handler of frame type {args.Type}.");
                    }

                    Next.Invoke(sender, args);
                }
            };

            frameClient.Disconnected += Disconnected;
        }
        
        public event EventHandler<MethodFrame> Received;

        public event EventHandler Disconnected;

        public void Send(Frame frame)
        {
            _frameClient.Send(frame);
        }

        public event EventHandler<Frame> Next;
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
                    if (Received == null)
                    {
                        throw new InvalidOperationException($"Missing subscription on {typeof(TMethod).FullName}.");
                    }

                    Received.Invoke(this, new MethodFrame<TMethod>(args.Channel, (TMethod)args.Method));
                }
            };

            methodFrameClient.Disconnected += Disconnected;
        }

        public event EventHandler<MethodFrame<TMethod>> Received;

        public event EventHandler Disconnected;

        public void Send(Frame frame)
        {
            _methodFrameClient.Send(frame);
        }
    }
}