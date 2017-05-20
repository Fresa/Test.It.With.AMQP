using Owin;

namespace Testing.Framework.Starters
{
    public interface IApplicationStarter
    {
        void Start(IAppBuilder applicationBuilder);
    }
}