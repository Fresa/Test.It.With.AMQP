namespace Test.It.With.RabbitMQ.Tests
{
    public class TestApplicationController : IWindowsServiceController
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