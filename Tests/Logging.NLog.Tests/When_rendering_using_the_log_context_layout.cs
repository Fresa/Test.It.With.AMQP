using System;
using FakeItEasy;
using NLog;
using NLog.Config;
using Should.Fluent;
using Test.It.With.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Log.It.With.NLog.Tests
{
    public class When_rendering_using_the_log_context_layout : XUnit2Specification
    {
        private NLogLogger _logger;
        private IWrite _writer;
        private bool _created;
        private bool _configurationObjectsCreatedAfterInstantiating;
        private ConfigurationItemCreator _defaultInstanceCreator;

        public When_rendering_using_the_log_context_layout(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Given()
        {
            _writer = A.Fake<IWrite>();

            _defaultInstanceCreator = ConfigurationItemFactory.Default.CreateInstance;
            ConfigurationItemFactory.Default.CreateInstance = type =>
            {
                _created = true;
                if (type == typeof(TestTarget))
                {
                    return new TestTarget(_writer);
                }
                if (type == typeof(NLogLogContextLayoutRenderer))
                {
                    return new NLogLogContextLayoutRenderer(new LogicalThreadContext());
                }
                if (type == typeof(XUnit2Target))
                {
                    return new XUnit2Target(Output);
                }
                return _defaultInstanceCreator(type);
            };

            _logger = new NLogLogger("TestType", new LogicalThreadContext());
            _configurationObjectsCreatedAfterInstantiating = _created;
            _logger.LogicalThread.Set("item1", "value1");

            OnDisposing += (sender, args) => Reset();
            LogManager.Configuration = LogManager.Configuration.Reload();
        }

        private void Reset()
        {
            ConfigurationItemFactory.Default.CreateInstance = _defaultInstanceCreator;
            LogManager.Shutdown();
        }

        protected override void When()
        {
            _logger.Info("Logging");
        }

        [Fact]
        public void It_should_have_writen_item1_but_not_item2()
        {
            A.CallTo(() => _writer.Write("item1=value1, item2=")).MustHaveHappened();
        }

        [Fact]
        public void It_should_not_have_created_nlog_configuration_objects_when_the_logger_has_not_logged_anything()
        {
            _configurationObjectsCreatedAfterInstantiating.Should().Be.False();
        }

        [Fact]
        public void It_should_have_created_nlog_configuration_objects()
        {
            _created.Should().Be.True();
        }
    }
}