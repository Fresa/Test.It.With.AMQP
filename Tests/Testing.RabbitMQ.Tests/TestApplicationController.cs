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

        public void Start()
        {
            _app.Start();
        }

        public void Stop()
        {
            _app.Stop();
        }
    }
}