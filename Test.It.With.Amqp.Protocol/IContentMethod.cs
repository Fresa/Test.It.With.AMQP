namespace Test.It.With.Amqp.Protocol
{
    public interface IContentMethod : IMethod
    {
        IContentHeader ContentHeader { get; }
        IContentBody[] ContentBodyFragments { get; }
        void SetContentHeader(IContentHeader contentHeader);
        void AddContentBody(IContentBody contentBody);
    }
}