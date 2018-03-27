namespace Test.It.With.Amqp.Protocol
{
    public interface IContentMethod : IMethod
    {
        IContentMethod SetContentHeader(IContentHeader contentHeader);
        IContentMethod AddContentBody(IContentBody contentBody);
    }

    public interface IContentMethod<out THeader> : IContentMethod 
        where THeader : IContentHeader
    {
        byte[] ContentBody { get; }
        IContentBody[] ContentBodyFragments { get; }
        THeader ContentHeader { get; }
    }
}