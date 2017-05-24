using Test.It.Specifications;
using Xunit;

namespace Log.It.With.NLog.Tests
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