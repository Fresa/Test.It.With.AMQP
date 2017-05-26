using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Test.It.Hosting.A.ConsoleApplication.Consoles
{
    internal class TestConsole : IConsole
    {
        private readonly ConcurrentQueue<string> _readableLines = new ConcurrentQueue<string>();
        private readonly ManualResetEventSlim _readLineWaiter = new ManualResetEventSlim();

        private readonly ConcurrentQueue<string> _output = new ConcurrentQueue<string>();
        private readonly object _outputLock = new object();

        public void WriteLine(string message)
        {
            lock (_outputLock)
            {
                if (OutputReceivedPrivate == null)
                {
                    _output.Enqueue(message);
                    return;
                }
                OutputReceivedPrivate.Invoke(this, message);
            }
        }

        private event EventHandler<string> OutputReceivedPrivate;
        public event EventHandler<string> OutputReceived
        {
            add
            {
                lock (_outputLock)
                {
                    while (_output.TryDequeue(out var message))
                    {
                        value.Invoke(this, message);
                    }
                    OutputReceivedPrivate += value;
                }
            }
            remove
            {
                lock (_outputLock)
                {
                    OutputReceivedPrivate -= value;
                }
            }
        }

        private const int ReadTimeoutInSeconds = 5;

        public string ReadLine()
        {
            while (true)
            {
                if (_readableLines.TryDequeue(out var readLine))
                {
                    return readLine;
                }

                if (_readLineWaiter.Wait(TimeSpan.FromSeconds(ReadTimeoutInSeconds)))
                {
                    continue;
                }

                throw new TimeoutException($"Waited for input for {ReadTimeoutInSeconds} seconds.");
            }
        }

        public string Title { get; set; }

        public void Input(string line)
        {
            _readableLines.Enqueue(line);
            _readLineWaiter.Set();
        }

        private event EventHandler<int> DisconnectedPrivate;
        public event EventHandler<int> Disconnected
        {
            add
            {
                lock (_disconnectLock)
                {
                    if (_disconnected)
                    {
                        value.Invoke(this, _exitCode);
                    }
                    DisconnectedPrivate += value;
                }
            }
            remove
            {
                lock (_disconnectLock)
                {
                    DisconnectedPrivate -= value;
                }
            }
        }

        private readonly object _disconnectLock = new object();
        private bool _disconnected;
        private int _exitCode;

        public void Disconnect(int exitCode)
        {
            lock (_disconnectLock)
            {
                _disconnected = true;
                _exitCode = exitCode;
            }
            DisconnectedPrivate?.Invoke(this, exitCode);
        }
    }
}