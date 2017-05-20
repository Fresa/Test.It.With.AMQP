namespace Testing.Framework.Fixtures
{
    public interface IClient
    {
        void Send<TMessage>(TMessage message);
    }
}