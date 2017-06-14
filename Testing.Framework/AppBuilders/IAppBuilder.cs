using Owin;

namespace Test.It.AppBuilders
{
    public interface IAppBuilder<in TController>
    {
        IAppBuilder WithController(TController controller);
    }
}