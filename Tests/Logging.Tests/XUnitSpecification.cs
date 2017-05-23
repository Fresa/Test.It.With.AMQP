using Test.It.Specifications;
using Xunit;

namespace Test.It.Tests
{
    public abstract class XUnitSpecification : Specification, IClassFixture<XUnitSpecification.StartUpFixture>
    {
        protected XUnitSpecification()
        {
            Setup();
        }

        internal class StartUpFixture { }
    }
}