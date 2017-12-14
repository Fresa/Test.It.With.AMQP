namespace Test.It.With.Amqp.Protocol
{
    public interface IContentMethod : IMethod
    {
        byte[] ContentBody { get; }
        void SetContentHeader(IContentHeader contentHeader);
        void AddContentBody(IContentBody contentBody);
    }
}