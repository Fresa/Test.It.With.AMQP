using Owin;

namespace Test.It.AppBuilders
{
    public interface IAppBuilder<in TClient>
    {
        IAppBuilder WithClient(TClient client);
    }
}