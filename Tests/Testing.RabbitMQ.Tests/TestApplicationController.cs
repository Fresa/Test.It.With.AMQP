using Test.It.Hosting.A.WindowsService;

namespace Test.It.With.RabbitMQ.Tests
{
    public class TestApplicationController : IWindowsService
    {
        private readonly TestApplicationSpecification _app;

        public TestApplicationController(TestApplicationSpecification app)
        {
            _app = app;
        }

        public int Start(params string[] args)
        {
            _app.Start();
            return 0;
        }

        public void Stop()
        {
            _app.Stop();
        }
    }
}