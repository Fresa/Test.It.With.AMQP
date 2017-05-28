using Owin;
using Test.It.AppBuilders;

namespace Test.It
{
    public class ClientProvidingAppBuilder<TClient> : IAppBuilder<TClient>
    {
        private readonly IAppBuilder _appBuilder;

        public ClientProvidingAppBuilder(IAppBuilder appBuilder)
        {
            _appBuilder = appBuilder;
        }

        public TClient Client { get; private set; }

        public IAppBuilder WithClient(TClient client)
        {
            Client = client;
            return _appBuilder;
        }
    }
}