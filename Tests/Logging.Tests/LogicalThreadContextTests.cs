using System.Threading.Tasks;
using Log.It;
using Should.Fluent;
using Xunit;

namespace Test.It.Tests
{
    public class When_setting_a_context_in_a_child_thread : XUnitSpecification
    {
        private string _secondItemInChildContext;
        private string _firstItemInParentContext;
        private string _secondItemInParentContext;
        private string _firstItemInChildContext;
        private string _overwrittenfirstItemInChildContext;
        private LogicalThreadContext _context;
        private Serializable _complexSerializableItemInChildContext;
        private Serializable _complexSerializableItemInParentContext;

        protected override void Given()
        {
            _context = new LogicalThreadContext();

            _context.Set("firstItem", "foo");
            _context.Set("secondItem", "fee");
            _context.Set("complexSerializableItem", new Serializable { Foo = "bar" });
        }

        protected override void When()
        {
            Task.Run(() =>
            {
                _firstItemInChildContext = _context.Get<string>("firstItem");
                _context.Set("firstItem", "bar");
                _overwrittenfirstItemInChildContext = _context.Get<string>("firstItem");
                _secondItemInChildContext = _context.Get<string>("secondItem");
                _complexSerializableItemInChildContext = _context.Get<Serializable>("complexSerializableItem");
            }).Wait();

            _firstItemInParentContext = _context.Get<string>("firstItem");
            _secondItemInParentContext = _context.Get<string>("secondItem");
            _complexSerializableItemInParentContext = _context.Get<Serializable>("complexSerializableItem");
        }

        [Fact]
        public void It_should_have_first_item_value_in_child_thread()
        {
            _firstItemInChildContext.Should().Equal("foo");
        }

        [Fact]
        public void It_should_have_overwritten_first_item_in_child_thread()
        {
            _overwrittenfirstItemInChildContext.Should().Equal("bar");
        }

        [Fact]
        public void It_should_have_original_first_item_value_in_parent_thread_after_child_thread_execution()
        {
            _firstItemInParentContext.Should().Equal("foo");
        }

        [Fact]
        public void It_should_have_original_second_item_value_in_child_thread()
        {
            _secondItemInChildContext.Should().Equal("fee");
        }

        [Fact]
        public void It_should_have_original_second_item_value_in_parent_thread()
        {
            _secondItemInParentContext.Should().Equal("fee");
        }

        [Fact]
        public void It_should_have_original_serializable_complex_item_value_in_child_thread()
        {
            _complexSerializableItemInChildContext.Foo.Should().Equal("bar");
        }

        [Fact]
        public void It_should_have_original_serializable_complex_item_value_in_parent_thread()
        {
            _complexSerializableItemInParentContext.Foo.Should().Equal("bar");
        }


        private class Serializable
        {
            public string Foo { get; set; }
        }
    }
}