using System;
using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.Amqp.MessageRouters
{
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
                throw new InvalidOperationException($"There is no router for frame type {message.Type}.");
            }

            _next.Handle(message);
        }
    }
}