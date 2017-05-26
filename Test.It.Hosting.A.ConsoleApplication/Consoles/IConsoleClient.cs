using System;

namespace Test.It.Hosting.A.ConsoleApplication.Consoles
{
    public interface IConsoleClient
    {
        event EventHandler<string> OutputReceived;
        event EventHandler<int> Disconnected;
        void Input(string message);
    }
}