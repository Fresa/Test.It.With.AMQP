using Owin;
using Test.It.AppBuilders;

namespace Test.It
{
    public class ControllerProvidingAppBuilder<TController> : IAppBuilder<TController>
    {
        private readonly IAppBuilder _appBuilder;

        public ControllerProvidingAppBuilder(IAppBuilder appBuilder)
        {
            _appBuilder = appBuilder;
        }

        public TController Controller { get; private set; }

        public IAppBuilder WithController(TController controller)
        {
            Controller = controller;
            return _appBuilder;
        }
    }
}