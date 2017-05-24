using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Should.Core.Exceptions;
using Should.Fluent;
using Test.It.ApplicationBuilders;
using Test.It.Fixtures;
using Test.It.MessageClient;
using Test.It.NetworkClient;
using Test.It.Specifications;
using Test.It.Starters;
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
            Client.Disconnected += (sender, args) => Done();

            Client.BufferReceived += (sender, s) => _got = s;
            Client.Send("test");
            Wait();
        }

        private void Wait()
        {
            if (_wait.WaitOne(TimeSpan.FromSeconds(5)) == false)
            {
                throw new TimeoutException(3);
            }
        }

        private void Done()
        {
            _wait.Set();
        }

        [Fact]
        public void It_should_have_responded()
        {
            _got.Should().Equal("test");
        }
    }

    public class TestConsoleClient : ITypedMessageClient<string, string>
    {
        private readonly Queue<string> _sentMessages = new Queue<string>(new []{"test"});

        public TestConsoleClient(TestConsole console)
        {
            // todo: make it async since the application tested is started async
            console.OnWriteLine(s => BufferReceived?.Invoke(this, s));
            console.OnReadLine(_sentMessages.Dequeue);
        }

        public event EventHandler<string> BufferReceived;
        public event EventHandler Disconnected;
        public void Send(string message)
        {
            _sentMessages.Enqueue(message);
        }

        public void TriggerDisconnected(int exitCode)
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }

    public interface IConsoleApplication
    {
        void Start(params string[] args);
    }

    public interface IEmitCtrlC
    {
        event EventHandler OnCtrlC;
    }

    public class DefaultConsoleMiddleware<TConsoleApplication> : IConsoleMiddleware
        where TConsoleApplication : class, IConsoleApplication
    {
        private readonly TConsoleApplication _consoleApplication;

        public DefaultConsoleMiddleware(TConsoleApplication consoleApplication)
        {
            _consoleApplication = consoleApplication;
            SetupCtrlCEvent(consoleApplication);
        }

        private void SetupCtrlCEvent(object app)
        {
            var iEmmitCtrlC = (IEmitCtrlC)app;
            if (iEmmitCtrlC != null)
            {
                iEmmitCtrlC.OnCtrlC += OnCtrlC;
            }
        }

        public void Start(params string[] args)
        {
            Task.Run(() =>
            {
                _consoleApplication.Start(args);
                OnClose?.Invoke(this, null);
            });
        }

        public event EventHandler OnCtrlC;
        public event EventHandler OnClose;
    }

    public interface IConsoleMiddleware
    {
        void Start(params string[] args);
        event EventHandler OnCtrlC;
        event EventHandler OnClose;
    }

    public class TestConsoleApplicationStarter : ConsoleApplicationStarter
    {
        private readonly TestConsoleApp _app;
        private readonly TestConsoleClient _console;

        public TestConsoleApplicationStarter(TestConsoleApp app, TestConsole console)
        {
            _app = app;
            _console = new TestConsoleClient(console);
            Starter = () =>
            {
                var exitCode = app.Start();
                _console.TriggerDisconnected(exitCode);
            };
        }

        protected override ITypedMessageClient<string, string> GetClient()
        {
            return _console;
        }

        //protected void Start(params string[] args)
        //{
        //    var exitCode = _app.Start(args);
        //    _console.TriggerDisconnected(exitCode);
        //}

        protected override Action Starter { get; }
    }

    public class TestConsoleApplicationBuilder : IApplicationBuilder
    {
        public IApplicationStarter CreateWith(ITestConfigurer configurer)
        {
            var console = new TestConsole();

            var app = new TestConsoleApp(container =>
            {
                container.Register<IConsole>(() => console);
                configurer.Configure(container);
            });
            return new TestConsoleApplicationStarter(app, console);
        }
    }

    public class XUnitConsoleApplicationSpecification<TFixture> : ConsoleApplicationSpecification<TFixture>, Xunit.IClassFixture<TFixture>
        where TFixture : class, IConsoleApplicationFixture, new()
    {
        public XUnitConsoleApplicationSpecification()
        {
            SetFixture(new TFixture());
        }
    }

    public interface IConsole
    {
        void WriteLine(string message);
        string ReadLine();
    }


    public class TestConsole : IConsole
    {
        private Func<string> _lineReader;
        private Action<string> _lineWriter;

        public void WriteLine(string message)
        {
            _lineWriter(message);
        }

        public string ReadLine()
        {
            return _lineReader();
        }

        public void OnWriteLine(Action<string> lineWriter)
        {
            _lineWriter = lineWriter;
        }

        public void OnReadLine(Func<string> lineReader)
        {
            _lineReader = lineReader;
        }
    }
}