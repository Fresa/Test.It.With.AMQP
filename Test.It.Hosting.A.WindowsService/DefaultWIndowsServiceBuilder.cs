using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.Hosting.A.WindowsService
{
    public abstract class DefaultWindowsServiceBuilder : IWindowsServiceBuilder
    {
        private static readonly WindowsServiceController WindowsServiceController = new WindowsServiceController();
        
        public abstract IWindowsService Create(ITestConfigurer configurer);
        
        public IApplicationStarter<IWindowsServiceClient> CreateWith(ITestConfigurer configurer)
        {            
            var application = Create(configurer);

            void InternalStarter()
            {
                application.Start();
                WindowsServiceController.Disconnect();
            }

            return new DefaultWindowsServiceStarter<IWindowsServiceClient>(InternalStarter, WindowsServiceController);
        }
    }
}