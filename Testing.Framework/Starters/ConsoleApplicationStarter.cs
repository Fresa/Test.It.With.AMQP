using Owin;
using Test.It.AppBuilders;
using Test.It.Fixtures;

namespace Test.It.Starters
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