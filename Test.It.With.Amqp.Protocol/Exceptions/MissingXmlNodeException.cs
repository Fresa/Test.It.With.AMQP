using System;
using System.Xml.XPath;
using Test.It.With.Amqp.Protocol.Extensions;

namespace Test.It.With.Amqp.Protocol.Exceptions
{
    public class MissingXmlNodeException : Exception
    {
        public MissingXmlNodeException(string name, IXPathNavigable currentNode)
            : base(BuildMessage(name, currentNode))
        {
        }

        private static string BuildMessage(string name, IXPathNavigable currentNode)
        {
            return $"Missing node: {currentNode.CreateNavigator().GetPath()}/{name}";
        }
    }
}