using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.Amqp.MessageClient
{  
    internal class MethodFrameClient<TMethod> : ITypedMessageClient<MethodFrame<TMethod>, IFrame> where TMethod : class, IMethod
    {
        private readonly ITypedMessageClient<MethodFrame, IFrame> _methodFrameClient;

        public MethodFrameClient( ITypedMessageClient<MethodFrame, IFrame> methodFrameClient)
        {
            _methodFrameClient = methodFrameClient;

            methodFrameClient.Received += args =>
            {
                if (args.Message.GetType() == typeof(TMethod))
                {
                    if (Received == null)
                    {
                        throw new InvalidOperationException($"Missing subscription on {typeof(TMethod).FullName}.");
                    }

                    Received.Invoke(new MethodFrame<TMethod>(args.Channel, (TMethod)args.Message));
                }
            };

            methodFrameClient.Disconnected += Disconnected;
        }

        public event Action<MethodFrame<TMethod>> Received;

        public event Action Disconnected;

        public void Send(IFrame frame)
        {
            _methodFrameClient.Send(frame);
        }
    }
}