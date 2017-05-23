using System;

namespace Test.It.Fixtures
{
    public interface IClient : IDisposable
    {
        void Send<TMessage>(TMessage message);
    }
}