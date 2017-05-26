using System;

namespace Test.It.Hosting.A.ConsoleApplication.Consoles
{
    internal class ConsoleClient : IConsoleClient
    {
        private readonly TestConsole _console;

        public ConsoleClient(TestConsole console)
        {
            _console = console;
        }

        public event EventHandler<string> OutputReceived
        {
            add => _console.OutputReceived += value;
            remove => _console.OutputReceived -= value;
        }

        public void Input(string message)
        {
            _console.Input(message);
        }
        
        public event EventHandler<int> Disconnected
        {
            add => _console.Disconnected += value;
            remove => _console.Disconnected -= value;
        }
    }
}