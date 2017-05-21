using System;

namespace Testing.Framework.Fixtures
{
    public interface IClient : IDisposable
    {
        void Send<TMessage>(TMessage message);
    }
}