namespace Test.It.Hosting.A.WindowsService.Tests
{
    public class XUnitWindowsServiceSpecification<TFixture> : WindowsServiceSpecification<TFixture>, Xunit.IClassFixture<TFixture>
        where TFixture : class, IWindowsServiceFixture, new()
    {
        public XUnitWindowsServiceSpecification()
        {
            SetFixture(new TFixture());
        }
    }
}