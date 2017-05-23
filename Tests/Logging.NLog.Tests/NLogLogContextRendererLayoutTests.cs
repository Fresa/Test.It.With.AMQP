using FakeItEasy;
using NLog.Config;
using Test.It.Specifications;
using Xunit;

namespace Log.It.With.NLog.Tests
{
    public class NLogLogContextRendererLayoutTests : XUnitSpecification
    {
        private NLogLogger _logger;
        private IWrite _writer;

        protected override void Given()
        {
            _writer = A.Fake<IWrite>();
            var defaultInstanceCreator = ConfigurationItemFactory.Default.CreateInstance;
            ConfigurationItemFactory.Default.CreateInstance = type =>
            {
                if (type == typeof(TestTarget))
                {
                    return new TestTarget(_writer);
                }
                if (type == typeof(NLogLogContextLayoutRenderer))
                {
                    return new NLogLogContextLayoutRenderer(new LogicalThreadContext());
                }
                return defaultInstanceCreator(type);
            };
            _logger = new NLogLogger("TestType", new LogicalThreadContext());
            _logger.LogicalThread.Set("item1", "value1");
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
    }

    public interface IWrite
    {
        void Write(string message);
    }

    public abstract class XUnitSpecification : Specification, IClassFixture<XUnitSpecification.StartUpFixture>
    {
        protected XUnitSpecification()
        {
            Setup();
        }

        internal class StartUpFixture { }
    }
}