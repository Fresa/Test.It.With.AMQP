using Test.It.Fixtures;
using Test.It.Hosting.A.ConsoleApplication;
using Test.It.Specifications;

namespace Test.It.Tests
{
    public class XUnitConsoleApplicationSpecification<TFixture> : ConsoleApplicationSpecification<TFixture>, Xunit.IClassFixture<TFixture>
        where TFixture : class, IConsoleApplicationFixture, new()
    {
        public XUnitConsoleApplicationSpecification()
        {
            SetFixture(new TFixture());
        }
    }
}