using Owin;

namespace Test.It.Starters
{
    public abstract class WindowsServiceStarter : IApplicationStarter
    {
        protected abstract IWindowsServiceController GetServiceController();

        public void Start(IAppBuilder appBuilder)
        {
            appBuilder.Use(GetServiceController());
        }
    }
}