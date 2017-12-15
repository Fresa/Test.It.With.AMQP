using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageClient
{  
    internal class MethodFrameClient<TMethod> : ITypedMessageClient<MethodFrame<TMethod>, Frame> where TMethod : class, IMethod
    {
        private readonly ITypedMessageClient<MethodFrame, Frame> _methodFrameClient;

        public MethodFrameClient(ITypedMessageClient<MethodFrame, Frame> methodFrameClient)
        {
            _methodFrameClient = methodFrameClient;

            methodFrameClient.Received += args =>
            {
                if (args.Method.GetType() == typeof(TMethod))
                {
                    if (Received == null)
                    {
                        throw new InvalidOperationException($"Missing subscription on {typeof(TMethod).FullName}.");
                    }

                    Received.Invoke(new MethodFrame<TMethod>(args.Channel, (TMethod)args.Method));
                }
            };

            methodFrameClient.Disconnected += Disconnected;
        }

        public event Action<MethodFrame<TMethod>> Received;

        public event Action Disconnected;

        public void Send(Frame frame)
        {
            _methodFrameClient.Send(frame);
        }
    }
}