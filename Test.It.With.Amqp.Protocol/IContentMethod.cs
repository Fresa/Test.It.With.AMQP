namespace Test.It.With.Amqp.Protocol
{
    public interface IContentMethod : IMethod
    {
        void SetContentHeader(IContentHeader contentHeader);
        void AddContentBody(IContentBody contentBody);
    }

    public interface IContentMethod<out THeader> : IContentMethod
    {
        byte[] ContentBody { get; }
        IContentBody[] ContentBodyFragments { get; }
        THeader ContentHeader { get; }
    }
}