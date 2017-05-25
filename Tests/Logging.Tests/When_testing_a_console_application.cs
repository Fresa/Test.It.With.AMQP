using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Should.Core.Exceptions;
using Should.Fluent;
using Test.It.Fixtures;
using Test.It.MessageClient;
using Test.It.NetworkClient;
using Xunit;
using TimeoutException = Should.Core.Exceptions.TimeoutException;

namespace Test.It.Tests
{
    public class When_testing_a_console_application : XUnitConsoleApplicationSpecification<StandardConsoleApplicationFixture<TestConsoleApplicationBuilder>>
    {
        private string _got;
        private AutoResetEvent _wait;

        protected override void When()
        {
            _wait = new AutoResetEvent(false);
            Client.Disconnected += (sender, args) => _wait.Set();

            Client.OutputReceived += (sender, message) => _got = message;
            Client.Input("test");
            Wait();
        }

        private void Wait()
        {
            if (_wait.WaitOne(TimeSpan.FromSeconds(500)) == false)
            {
                throw new TimeoutException(5000);
            }
        }
        
        [Fact]
        public void It_should_have_responded()
        {
            _got.Should().Equal("test");
        }
    }

    internal class ConsoleClient : IConsoleClient
    {
        private readonly TestConsole _console;

        public ConsoleClient(TestConsole console)
        {
            _console = console;

            _console.OutputReceived += OutputReceived;
            _console.Disconnected += Disconnected;
        }

        // todo: is null if no receivers are registered. Should defer.
        public event EventHandler<string> OutputReceived;

        public void Input(string message)
        {
            _console.Input(message);
        }

        public event EventHandler<int> Disconnected;
    }

    public class TestConsole : IConsole
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
            if (_readableLines.TryDequeue(out var readLine))
            {
                return readLine;
            }

            if (_readLineWaiter.Wait(TimeSpan.FromSeconds(ReadTimeoutInSeconds)))
            {
                // ReSharper disable once TailRecursiveCall (Cannot loop since this is an async dependent. Getting a read line event does not mean there is a message ready to be read by THIS instance; it's a race condition.)
                return ReadLine();
            }

            throw new System.TimeoutException($"Waited for input for {ReadTimeoutInSeconds} seconds.");
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