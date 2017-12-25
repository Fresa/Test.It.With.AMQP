using System;
using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageRouters
{
    internal abstract class FrameRouter : IHandle<IFrame>
    {
        private readonly IHandle<IFrame> _next;
        public abstract void Handle(IFrame message);

        protected FrameRouter(IHandle<IFrame> next)
        {
            _next = next;
        }

        protected void Next(IFrame message)
        {
            if (_next == null)
            {
                throw new InvalidOperationException($"There is no router for frame {message}.");
            }

            _next.Handle(message);
        }
    }
}