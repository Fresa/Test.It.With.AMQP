using System;
using System.IO;
using System.Text;
using NLog.Config;
using RabbitMQ.Client;
using Should.Fluent;
using Test.It.Specifications;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Test.It.With.XUnit;
using Xunit;
using Xunit.Abstractions;
using ILogger = Log.It.ILogger;
using LogFactory = Log.It.LogFactory;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
    {
        private RabbitMqTestFramework2.ClientEnvelope<TestMessage> _testMessagePublished;

        public When_publishing_a_message(ITestOutputHelper output) : base(output)
        {

        }

        protected override void Given(IServiceContainer container)
        {
            var defaultInstanceCreator = ConfigurationItemFactory.Default.CreateInstance;
            ConfigurationItemFactory.Default.CreateInstance = type =>
            {
                if (type == typeof(XUnit2Target))
                {
                    return new XUnit2Target(Output);
                }
                return defaultInstanceCreator(type);
            };
            
            var rabbitMqTestServer = new RabbitMqTestFramework2(new NewtonsoftSerializer(Encoding.UTF8), new Lazy<IConnectionFactory>(() => new ConnectionFactory()));
            //rabbitMqTestServer.On<TestMessage>(envelope =>
            //{
            //    _testMessagePublished = envelope;
            //    Client.Disconnect();
            //});

            container.Register(() => rabbitMqTestServer.ConnectionFactory);
        }

        [Fact]
        public void It_should_have_published_the_message()
        {
            _testMessagePublished.Should().Not.Be.Null();
        }
    }

    internal class TestOutputHelperTextWriter : TextWriter
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private string _value = string.Empty;
        private readonly object _lock = new object();

        public TestOutputHelperTextWriter(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public override void WriteLine(string value)
        {
            lock (_lock)
            {
                _value += value;
            }
            WriteLine();
        }

        public override void WriteLine()
        {
            lock (_lock)
            {
                _testOutputHelper.WriteLine(_value);
                _value = string.Empty; 
            }
        }

        public override void Write(char value)
        {
            lock (_lock)
            {
                _value += value;
            }
        }

        public override Encoding Encoding { get; } = Encoding.UTF8;
    }
}
