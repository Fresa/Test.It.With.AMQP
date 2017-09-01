using System;
using System.Xml.XPath;
using Test.It.With.RabbitMQ.Extensions;

namespace Test.It.With.RabbitMQ.Protocol.Exceptions
{
    public class ToManyXmlNodesException : Exception
    {
        public ToManyXmlNodesException(string name, IXPathNavigable currentNode, int expectedCount, int currentCount)
            : base(BuildMessage(name, currentNode, expectedCount, currentCount))
        {
        }

        private static string BuildMessage(string name, IXPathNavigable currentNode, int expectedCount, int currentCount)
        {
            return $"To many nodes {currentNode.CreateNavigator().GetPath()}/{name}. Expected {expectedCount}, got {currentCount}.";
        }
    }
}