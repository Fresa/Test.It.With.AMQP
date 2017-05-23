using Owin;

namespace Test.It.Starters
{
    public interface IApplicationStarter
    {
        void Start(IAppBuilder applicationBuilder);
    }
}