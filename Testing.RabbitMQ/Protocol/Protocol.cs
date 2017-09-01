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
            var amqpNodes = definition.SelectNodes("amqp");
            if (amqpNodes.IsNullOrEmpty())
            {
                throw new MissingXmlNodeException("amqp", definition);
            }

            if (amqpNodes.Count > 1)
            {
                throw new ToManyXmlNodesException("amqp", definition, 1, amqpNodes.Count);
            }

            var amqpNode = amqpNodes.Cast<XmlElement>().First();
            
            Major = amqpNode.GetMandatoryAttribute<int>("major");
            Minor = amqpNode.GetMandatoryAttribute<int>("minor");
            Revision = amqpNode.GetMandatoryAttribute<int>("revision");

            Constants = ParseConstants(amqpNode);
            Domains = ParseDomains(amqpNode, this);
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
            if (constantNodes.IsNullOrEmpty())
            {
                throw new MissingXmlNodesException("constant", amqpNode);
            }

            var constants = new Dictionary<string, Constant>();
            foreach (XmlElement constantNode in constantNodes)
            {
                var name = constantNode.GetMandatoryAttribute<string>("name");
                var value = constantNode.GetMandatoryAttribute<int>("value");

                var constant = new Constant(name, value)
                {
                    Class = constantNode.GetOptionalAttribute<string>("class"),
                    Documentation = GetFirstDocumentation(constantNode)
                };

                constants.Add(name, constant);
            }

            return constants;
        }

        private static IDictionary<string, Domain> ParseDomains(XmlNode amqpNode, Protocol protocol)
        {
            var domainNodes = amqpNode.SelectNodes("domain");
            if (domainNodes.IsNullOrEmpty())
            {
                throw new MissingXmlNodeException("domain", amqpNode);
            }

            var domains = new Dictionary<string, Domain>();
            foreach (XmlElement domainNode in domainNodes)
            {
                var name = domainNode.GetMandatoryAttribute<string>("name");
                var type = domainNode.GetMandatoryAttribute<string>("type");

                var domain = new Domain(name, type)
                {
                    Label = domainNode.GetOptionalAttribute<string>("label"),
                    Documentation = GetFirstDocumentation(domainNode),
                    Rules = ParseRules(domainNode),
                    Asserts = ParseAsserts(domainNode, protocol)
                };

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
                    Documentation = GetFirstDocumentation(ruleNode),
                    ScenarioDocumentation = GetFirstDocumentationType(ruleNode, ScenarioDocumentation)
                };

                yield return rule;
            }
        }

        private static IEnumerable<Assert> ParseAsserts(XmlNode node, Protocol protocol)
        {
            var assertNodes = node.SelectNodes("assert");
            if (assertNodes.IsNull())
            {
                yield break;
            }

            foreach (XmlElement assertNode in assertNodes)
            {
                var check = assertNode.GetMandatoryAttribute<string>("check");

                var method = assertNode.GetOptionalAttribute<string>("method");
                var fieldResolver = new Lazy<Field>(() => null);
                if (string.IsNullOrEmpty(method) == false)
                {
                    var field = assertNode.GetMandatoryAttribute<string>("field");

                    fieldResolver = new Lazy<Field>(() =>
                    {
                        var matchingMethods =
                            protocol.Classes.SelectMany(
                                pair => pair.Value.Methods.Where(valuePair => valuePair.Key == method)).ToList();
                        if (matchingMethods.Any() == false)
                        {
                            throw new MissingMethodException("unknown", method);
                        }

                        var matchingFields =
                            matchingMethods.SelectMany(
                                valuePair => valuePair.Value.Fields.Where(keyValuePair => keyValuePair.Key == field)).ToList();
                        if (matchingFields.Any() == false)
                        {
                            throw new MissingFieldException(method, field);
                        }

                        return matchingFields.First().Value;
                    });
                }

                var rule = new Assert(check, fieldResolver)
                {
                    Value = assertNode.GetOptionalAttribute<string>("value"),
                };
                
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
                    Documentation = GetFirstDocumentation(classNode),
                    GrammarDocumentation = GetFirstDocumentationType(classNode, GrammarDocumentation),
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
                    Documentation = GetFirstDocumentation(methodNode),
                    Rules = ParseRules(methodNode),
                    Responses = ParseResponse(methodNode, protocol),
                    Fields = ParseFields(methodNode, protocol),
                    Chassis = ParseChassis(methodNode)
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
                        throw new MissingXmlAttributeException("domain or type", fieldNode);
                    }
                }

                if (protocol.Domains.ContainsKey(domain) == false)
                {
                    throw new XmlException($"Missing domain '{domain}'. Found {string.Join(", ", protocol.Domains.Keys.Select(key => $"'{key}'"))}");
                }

                var field = new Field(name, protocol.Domains[domain])
                {
                    Label = fieldNode.GetOptionalAttribute<string>("label"),
                    Documentation = GetFirstDocumentation(fieldNode),
                    Rules = ParseRules(fieldNode),
                    Asserts = ParseAsserts(fieldNode, protocol)
                };

                fields.Add(name, field);
            }
            return fields;
        }

        private static IReadOnlyDictionary<string, Response> ParseResponse(XmlNode node, Protocol protocol)
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

                var methodResolver = new Lazy<Method>(() =>
                {
                    var matchingMethods = protocol.Classes
                        .SelectMany(pair => pair.Value.Methods.Where(valuePair => valuePair.Key == name)).ToList();

                    if (matchingMethods.Any() == false)
                    {
                        throw new MissingMethodException("unknown", name);
                    }

                    return matchingMethods.First().Value;
                });

                var response = new Response(methodResolver);
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
                    (ChassisName)Enum.Parse(typeof(ChassisName), element.GetMandatoryAttribute<string>("name"), true),
                    element.GetMandatoryAttribute<string>("implement") == "MUST"));
        }

        private static string GetFirstDocumentation(XmlNode node)
        {
            return node
                .SelectNodes("doc")
                .CastOrEmptyList<XmlElement>()
                .Where(xmlNode => xmlNode.HasAttribute("type") == false)
                .Select(element => element.InnerText)
                .FirstOrDefault();
        }

        private static string GetFirstDocumentationType(XmlNode node, string type)
        {
            return node
                .SelectNodes("doc")
                .CastOrEmptyList<XmlElement>()
                .Where(xmlNode => xmlNode.HasAttribute("type") && xmlNode.GetAttribute("type") == type)
                .Select(element => element.InnerText)
                .FirstOrDefault();
        }

        private const string ScenarioDocumentation = "scenario";
        private const string GrammarDocumentation = "grammar";
    }
}