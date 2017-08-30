using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Test.It.With.RabbitMQ.Extensions;
using Test.It.With.RabbitMQ.Protocol.Exceptions;

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
            Classes = ParseClasses(amqpNode, this);
        }
        
        public int Major { get; }
        public int Minor { get; }
        public int Revision { get; }
        public IDictionary<string, Constant> Constants { get; }
        public IDictionary<string, Domain> Domains { get; }
        public IDictionary<string, Class> Classes { get; }

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
                var name = ruleNode.GetMandatoryAttribute<string>("name");

                var rule = new Rule(name)
                {
                    Documentation = GetDocumentation(ruleNode),
                    ScenarioDocumentation = GetDocumentation(ruleNode, DocumentationTypes.Grammar),
                };

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
                    rule.Method = assertNode.GetAttribute("method");
                }

                if (assertNode.HasAttribute("field"))
                {
                    rule.Field = assertNode.GetAttribute("field");
                }

                yield return rule;
            }
        }

        private static IDictionary<string, Class> ParseClasses(XmlNode node, Protocol protocol)
        {
            var classNodes = node.SelectNodes("class");
            if (classNodes.IsNullOrEmpty())
            {
                throw new MissingXmlNodeException("class", node);
            }

            var classes = new Dictionary<string, Class>();
            foreach (XmlElement classNode in classNodes)
            {
                var name = classNode.GetMandatoryAttribute<string>("name");
                var handler = classNode.GetMandatoryAttribute<string>("handler");
                var index = classNode.GetMandatoryAttribute<int>("index");

                var @class = new Class(name, handler, index, ParseMethods(classNode, protocol))
                {
                    Label = classNode.GetOptionalAttribute<string>("label"),
                    Documentation = GetDocumentation(classNode),
                    GrammarDocumentation = GetGrammarDocumentation(classNode),
                    Chassis = ParseChassis(classNode)
                };

                classes.Add(name, @class);
            }
            return classes;
        }

        private static IReadOnlyDictionary<string, Method> ParseMethods(XmlNode node, Protocol protocol)
        {
            var methodNodes = node.SelectNodes("method");
            if (methodNodes.IsNullOrEmpty())
            {
                throw new MissingXmlNodeException("method", node);
            }

            var methods = new Dictionary<string, Method>();
            foreach (XmlElement methodNode in methodNodes)
            {
                var name = methodNode.GetMandatoryAttribute<string>("name");
                var index = methodNode.GetMandatoryAttribute<int>("index");
                var label = methodNode.GetOptionalAttribute<string>("label");

                var method = new Method(name, index)
                {
                    Synchronous = methodNode.GetOptionalAttribute<int>("synchronous") == 1,
                    Label = label,
                    Documentation = GetDocumentation(methodNode),
                    Rules = ParseRules(methodNode),
                    Responses = ParseResponse(methodNode),
                    Fields = ParseFields(methodNode, protocol)
                };
                methods.Add(name, method);
            }
            return methods;
        }

        private static IReadOnlyDictionary<string, Field> ParseFields(XmlNode node, Protocol protocol)
        {
            var fieldNodes = node.SelectNodes("field");
            var fields = new Dictionary<string, Field>();
            if (fieldNodes.IsNullOrEmpty())
            {
                return fields;
            }

            foreach (XmlElement fieldNode in fieldNodes)
            {
                var name = fieldNode.GetMandatoryAttribute<string>("name");
                var domain = fieldNode.GetOptionalAttribute<string>("domain");
                if (string.IsNullOrEmpty(domain))
                {
                    domain = fieldNode.GetOptionalAttribute<string>("type");
                    if (string.IsNullOrEmpty(domain))
                    {
                        throw new MissingXmlAttributeException("domain and type", fieldNode);
                    }
                }

                if (protocol.Domains.ContainsKey(domain) == false)
                {
                    throw new XmlException($"Missing domain '{domain}'. Found {string.Join(", ", protocol.Domains.Keys.Select(key => $"'{key}'"))}");
                }

                var field = new Field(name, protocol.Domains[domain])
                {
                    Label = fieldNode.GetOptionalAttribute<string>("label"),
                    Documentation = GetDocumentation(fieldNode),
                    Rules = ParseRules(fieldNode),
                    Asserts = ParseAsserts(fieldNode)
                };

                fields.Add(name, field);
            }
            return fields;
        }

        private static IReadOnlyDictionary<string, Response> ParseResponse(XmlNode node)
        {
            var responseNodes = node.SelectNodes("response");
            var responses = new Dictionary<string, Response>();
            if (responseNodes.IsNullOrEmpty())
            {
                return responses;
            }

            foreach (XmlElement responseNode in responseNodes)
            {
                var name = responseNode.GetMandatoryAttribute<string>("name");
                var response = new Response(name);
                responses.Add(name, response);
            }

            return responses;
        }

        private static IEnumerable<Chassis> ParseChassis(XmlNode node)
        {
            return node
                .SelectNodes("chassis")
                .CastOrEmptyList<XmlElement>()
                .Select(element => new Chassis(
                    (ChassisName)Enum.Parse(typeof(ChassisName), element.GetMandatoryAttribute<string>("name")),
                    element.GetMandatoryAttribute<string>("implement") == "MUST"));
        }

        private static string GetDocumentation(XmlNode node, params DocumentationTypes[] attributeNames)
        {
            return node
                .SelectNodes("doc")
                .CastOrEmptyList<XmlElement>()
                .Where(xmlNode => attributeNames
                    .Select(documentationType => Enum
                        .GetName(typeof(DocumentationTypes), documentationType)
                        .SeperateOnCase('-')
                        .ToLower())
                    .All(xmlNode.HasAttribute))
                .Select(element => element.InnerText)
                .FirstOrDefault();
        }

        private enum DocumentationTypes
        {
            Scenario,
            Grammar
        }

        private static string GetGrammarDocumentation(XmlNode node)
        {
            return node
                .SelectNodes("doc")
                .CastOrEmptyList<XmlElement>()
                .Where(xmlNode => xmlNode.HasAttribute("grammar"))
                .Select(element => element.InnerText)
                .FirstOrDefault();
        }
    }
}