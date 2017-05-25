using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.ApplicationBuilders
{
    public abstract class BaseConsoleApplicationBuilder : IConsoleApplicationBuilder<IConsoleClient>
    {
        protected abstract IServiceContainer UseServiceContainer();

        protected abstract IApplicationStarter<IConsoleClient> GetApplicationStarter();

        public IApplicationStarter<IConsoleClient> CreateWith(ITestConfigurer configurer)
        {
            configurer.Configure(UseServiceContainer());
            return GetApplicationStarter();
        }
    }
}