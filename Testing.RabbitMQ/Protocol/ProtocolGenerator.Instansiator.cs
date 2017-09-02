using System;
using System.Xml;
using Validation;

namespace Test.It.With.RabbitMQ.Protocol
{
    //internal partial class ProtocolGenerator
    //{
    //    public ProtocolGenerator(Protocol protocol)
    //    {
    //        _protocol = protocol;
    //    }
    //}

    public struct ShortString
    {
        public ShortString(string str)
        {
            Requires.NotNull(str, nameof(str));
            Requires.Range(str.Length <= 127, nameof(str));

       
        }
    }

}