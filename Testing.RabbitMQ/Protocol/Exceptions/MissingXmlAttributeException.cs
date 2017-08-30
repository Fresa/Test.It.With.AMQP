using System;
using System.Xml.XPath;
using Test.It.With.RabbitMQ.Extensions;

namespace Test.It.With.RabbitMQ.Protocol.Exceptions
{
    public class MissingXmlAttributeException : Exception
    {
        public MissingXmlAttributeException(string name, IXPathNavigable currentNode)
            : base(BuildMessage(name, currentNode))
        {
        }

        private static string BuildMessage(string name, IXPathNavigable currentNode)
        {
            return $"Missing attribute: {currentNode.CreateNavigator().GetPath()}/@{name}";
        }
    }
}