using System;

namespace Test.It
{
    public interface IConsoleClient
    {
        event EventHandler<string> OutputReceived;
        event EventHandler<int> Disconnected;
        void Input(string message);
    }
}