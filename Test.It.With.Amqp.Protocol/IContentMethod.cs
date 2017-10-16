namespace Test.It.With.Amqp.Protocol
{
    public interface IContentMethod : IMethod
    {
        void SetContentHeader(IContentHeader contentHeader);
        void AddContentBody(IContentBody contentBody);
    }
}