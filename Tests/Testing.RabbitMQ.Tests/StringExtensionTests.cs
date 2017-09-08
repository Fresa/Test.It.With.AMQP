using Should.Fluent;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_transforming_a_string_to_pascal_case : XUnit2Specification
    {
        private string _str;
        private string _pascalCaseString;

        protected override void Given()
        {
            _str = "this-is-a-CuStomText";
        }

        protected override void When()
        {
            _pascalCaseString = _str.ToPascalCase('-');
        }

        [Fact]
        public void It_should_have_transformed_the_string_to_pascal_case()
        {
            _pascalCaseString.Should().Equal("ThisIsACustomtext");
        }
    }
}