namespace Test.It.Hosting.A.ConsoleApplication.Tests
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