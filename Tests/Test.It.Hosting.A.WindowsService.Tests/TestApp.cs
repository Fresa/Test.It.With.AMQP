namespace Test.It.Hosting.A.WindowsService.Tests
{
    internal class TestApp : ITestApp
    {
        public bool HaveStarted { get => false; set {} }
    }
}