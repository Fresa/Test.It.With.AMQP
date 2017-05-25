using System;

namespace Test.It
{
    public interface IServerConsoleClient : IConsoleClient
    {
        void Disconnect(int exitCode);
    }

    public interface IConsoleClient
    {
        event EventHandler<string> OutputReceived;
        event EventHandler<int> Disconnected;
        void Input(string message);
    }
}