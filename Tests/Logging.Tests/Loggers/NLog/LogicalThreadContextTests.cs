using System.Threading.Tasks;
using Logging.Loggers.NLog;
using Should.Fluent;
using Testing.Framework.Specifications;
using Xunit;

namespace Logging.Tests.Loggers.NLog
{
    public class When_setting_a_context_in_a_child_thread : Specification
    {
        private string _parentItemInChildContext;
        private string _commonItemInParentContext;
        private string _parentItemInParentContext;
        private string _commonItemInChildContext;
        private string _overwrittenCommonItemInChildContext;

        public When_setting_a_context_in_a_child_thread()
        {
            Setup();
        }

        protected override void Given()
        {
            NLogLogicalThreadContext.Set("parentItem", "123");
            NLogLogicalThreadContext.Set("commonItem", "abc");
        }

        protected override void When()
        {
            Task.Run(() =>
            {
                _commonItemInChildContext = NLogLogicalThreadContext.Get("commonItem");
                NLogLogicalThreadContext.Set("commonItem", "def");
                _overwrittenCommonItemInChildContext = NLogLogicalThreadContext.Get("commonItem");
                _parentItemInChildContext = NLogLogicalThreadContext.Get("parentItem");
            }).Wait();

            _commonItemInParentContext = NLogLogicalThreadContext.Get("commonItem");
            _parentItemInParentContext = NLogLogicalThreadContext.Get("parentItem");
        }

        [Fact]
        public void It_should_have_common_item_context_in_child_thread()
        {
            _commonItemInChildContext.Should().Equal("abc");
        }

        [Fact]
        public void It_should_have_overwritten_common_item_context_in_child_thread()
        {
            _overwrittenCommonItemInChildContext.Should().Equal("def");
        }

        [Fact]
        public void It_should_have_parent_item_context_in_child_thread()
        {
            _parentItemInChildContext.Should().Equal("123");
        }

        [Fact]
        public void It_should_have_original_parent_item_context_in_parent_thread()
        {
            _parentItemInParentContext.Should().Equal("123");
        }

        [Fact]
        public void It_should_have_original_common_item_context_in_parent_thread_after_child_thread_execution()
        {
            _commonItemInParentContext.Should().Equal("abc");
        }
    }
}