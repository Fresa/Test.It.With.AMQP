using Owin;
using Test.It.AppBuilders;

namespace Test.It.Starters
{
    public interface IApplicationStarter
    {
        void Start(IAppBuilder applicationBuilder);
    }

    public interface IApplicationStarter<out TClient>
    {
        void Start(IAppBuilder<TClient> applicationBuilder);
    }
}