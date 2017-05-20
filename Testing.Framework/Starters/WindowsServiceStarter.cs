using Owin;

namespace Testing.Framework.Starters
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