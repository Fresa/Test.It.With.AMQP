using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using JetBrains.Annotations;

namespace Test.It.With.RabbitMQ.Protocol
{
    internal class Protocol
    {
        public Protocol(XmlNode definition)
        {
            var amqpNode = definition.SelectSingleNode("amqp");
            if (amqpNode == null)
            {
                throw new NotSupportedException("Missing amqp root definition");
            }

            if (amqpNode.Attributes == null)
            {
                throw new NotSupportedException("Missing amq protocol version");
            }

            Major = int.Parse(amqpNode.Attributes["major"].Value);
            Minor = int.Parse(amqpNode.Attributes["minor"].Value);
            Revision = int.Parse(amqpNode.Attributes["revision"].Value);

            Constants = ParseConstants(amqpNode);
            Domains = ParseDomains(amqpNode);
        }

        public int Major { get; }
        public int Minor { get; }
        public int Revision { get; }
        public IDictionary<string, Constant> Constants { get; }
        public IDictionary<string, Domain> Domains { get; }

        private static IDictionary<string, Constant> ParseConstants(XmlNode amqpNode)
        {
            var constantNodes = amqpNode.SelectNodes("constant");
            if (constantNodes == null || constantNodes.Count == 0)
            {
                throw new NotSupportedException("Missing constants");
            }

            var constants = new Dictionary<string, Constant>();
            foreach (XmlElement constantNode in constantNodes)
            {
                var name = constantNode.GetAttribute("name");
                if (string.IsNullOrEmpty(name))
                {
                    throw new NotSupportedException("Missing constant name.");
                }
                var value = int.Parse(constantNode.GetAttribute("value"));

                var constant = new Constant(name, value);

                if (constantNode.HasAttribute("class"))
                {
                    constant.Class = constantNode.GetAttribute("class");
                }

                var docNode = constantNode.SelectSingleNode("doc");
                if (docNode != null)
                {
                    constant.Documentation = docNode.InnerText;
                }

                constants.Add(name, constant);
            }

            return constants;
        }



        private static IDictionary<string, Domain> ParseDomains(XmlNode amqpNode)
        {
            var domainNodes = amqpNode.SelectNodes("domain");
            if (domainNodes.IsNullOrEmpty())
            {
                throw new MissingXmlNodeException("domain", amqpNode);
            }

            var domains = new Dictionary<string, Domain>();
            foreach (XmlElement domainNode in domainNodes)
            {
                var name = domainNode.GetAttribute("name");
                if (string.IsNullOrEmpty(name))
                {
                    throw new MissingXmlAttributeException("name", domainNode);
                }

                var type = domainNode.GetAttribute("type");
                if (string.IsNullOrEmpty(type))
                {
                    throw new MissingXmlAttributeException("type", domainNode);
                }

                var domain = new Domain(name, type);

                if (domainNode.HasAttribute("label"))
                {
                    domain.Label = domainNode.GetAttribute("label");
                }

                var docNode = domainNode.SelectSingleNode("doc");
                if (docNode != null)
                {
                    domain.Documentation = docNode.InnerText;
                }

                domain.Rules = ParseRules(domainNode);
                domain.Asserts = ParseAsserts(domainNode);

                domains.Add(name, domain);
            }
            return domains;
        }

        private static IEnumerable<Rule> ParseRules(XmlNode domainNode)
        {
            var ruleNodes = domainNode.SelectNodes("rule");
            if (ruleNodes.IsNull())
            {
                yield break;
            }

            foreach (XmlElement ruleNode in ruleNodes)
            {
                var name = ruleNode.GetAttribute("name");
                if (string.IsNullOrEmpty(name))
                {
                    throw new MissingXmlAttributeException("name", domainNode);
                }

                var rule = new Rule(name);

                var docNode = ruleNode.SelectSingleNode("doc");
                if (docNode != null)
                {
                    rule.Documentation = docNode.InnerText;
                }

                yield return rule;
            }
        }

        private static IEnumerable<Assert> ParseAsserts(XmlNode node)
        {
            var assertNodes = node.SelectNodes("assert");
            if (assertNodes.IsNull())
            {
                yield break;
            }

            foreach (XmlElement assertNode in assertNodes)
            {
                var check = assertNode.GetAttribute("check");
                if (string.IsNullOrEmpty(check))
                {
                    throw new MissingXmlAttributeException("check", assertNode);
                }

                var rule = new Assert(check);

                if (assertNode.HasAttribute("value"))
                {
                    rule.Value = assertNode.GetAttribute("value");
                }

                if (assertNode.HasAttribute("method"))
                {
                    rule.Value = assertNode.GetAttribute("method");
                }

                if (assertNode.HasAttribute("field"))
                {
                    rule.Value = assertNode.GetAttribute("field");
                }

                yield return rule;
            }
        }
    }

    public static class XPathNavigatorExtensions
    {
        public static string GetPath(this XPathNavigator navigator)
        {
            var path = new StringBuilder();
            for (var node = navigator.UnderlyingObject as XmlNode; node != null; node = node.ParentNode)
            {
                var append = "/" + path;

                if (node.ParentNode != null && node.ParentNode.ChildNodes.Count > 1)
                {
                    append += "[";

                    var index = 0;
                    var sibling = node;
                    while (sibling.PreviousSibling != null)
                    {
                        index++;
                        sibling = sibling.PreviousSibling;
                    }

                    append += index;
                    append += "]";
                }

                path.Insert(0, append);
            }

            return path.ToString();
        }
    }

    public static class XmlNodeListExtensions
    {
        [ContractAnnotation("null => true")]
        public static bool IsNullOrEmpty(this XmlNodeList list)
        {
            return list == null || list.Count == 0;
        }

        [ContractAnnotation("null => true")]
        public static bool IsNull(this XmlNodeList list)
        {
            return list == null;
        }

    }

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