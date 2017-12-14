using System;

namespace Test.It.With.Amqp.Protocol
{
    public interface IContentMethod : IMethod
    {
        IContentBody[] ContentBodyFragments { get; }
        void SetContentHeader(IContentHeader contentHeader);
        void AddContentBody(IContentBody contentBody);
    }
}