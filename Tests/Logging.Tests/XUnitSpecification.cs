using Testing.Framework.Specifications;
using Xunit;

namespace Logging.Tests
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