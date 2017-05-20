using Owin;
using Testing.Framework.AppBuilders;
using Testing.Framework.Fixtures;

namespace Testing.Framework.Starters
{
    public abstract class ConsoleApplicationStarter : IApplicationStarter
    {
        protected abstract IClient GetClient(params object[] args);

        public void Start(IAppBuilder applicationBuilder)
        {
            applicationBuilder.Use(GetClient);
        }
    }
}