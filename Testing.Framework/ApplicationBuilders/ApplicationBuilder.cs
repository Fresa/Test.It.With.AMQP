using Testing.Framework.Specifications;
using Testing.Framework.Starters;

namespace Testing.Framework.ApplicationBuilders
{
    public abstract class ApplicationBuilder : IApplicationBuilder
    {
        protected abstract IServiceContainer UseServiceContainer();

        protected abstract IApplicationStarter GetApplicationStarter();

        public IApplicationStarter CreateWith(ITestConfigurer configurer)
        {
            configurer.Configure(UseServiceContainer());
            return GetApplicationStarter();
        }
    }
}