using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Should.Fluent;
using Test.It.With.Amqp.Protocol.Definitions;
using Test.It.With.XUnit;
using Xunit;

namespace Test.It.With.Amqp.ProtocolGenerator
{
    public class When_parsing_the_amqp_0_9_1_protocol : XUnit2Specification
    {
        private XmlDocument _definition;
        private Protocol.Definitions.Protocol _protocol;

        protected override void Given()
        {
            var path = Path.Combine(Environment.CurrentDirectory, @"amqp0-9-1.xml");
            _definition = new XmlDocument();
            _definition.Load(path);
        }

        protected override void When()
        {
            _protocol = new Amqp.Protocol.Definitions.Protocol(_definition);
        }

        [Fact]
        public void It_should_have_parsed_version()
        {
            _protocol.Major.Should().Equal(0);
            _protocol.Minor.Should().Equal(9);
            _protocol.Revision.Should().Equal(1);
        }

        [Fact]
        public void It_should_have_parsed_constants()
        {
            _protocol.Constants.Should().Count.Exactly(24);
            _protocol.Constants.Should().Contain.One(pair =>
                pair.Key == "content-too-large" &&
                pair.Value.Value == 311 &&
                pair.Value.Class == "soft-error" &&
                pair.Value.Documentation.Contains(@"The client attempted to transfer content larger than the server could") &&
                pair.Value.Name == "content-too-large");
        }

        [Fact]
        public void It_should_have_parsed_domains()
        {
            _protocol.Domains.Should().Count.Exactly(24);
            _protocol.Domains.Should().Contain.One(pair =>
                pair.Key == "exchange-name" &&
                pair.Value.Name == "exchange-name" &&
                pair.Value.Type == "shortstr" &&
                pair.Value.Asserts.Count() == 2 &&
                pair.Value.Asserts.Any(assert => assert.Value == "127" && assert.Check == "length") &&
                pair.Value.Label == "exchange name" &&
                !pair.Value.Rules.Any() &&
                pair.Value.Documentation.Trim().StartsWith("The exchange name is a client-selected string that identifies the exch")
            );
        }

        [Fact]
        public void It_should_have_parsed_classes()
        {
            _protocol.Classes.Should().Count.Exactly(6);
            _protocol.Classes.Should().Contain.One(pair =>
                pair.Key == "connection" &&
                pair.Value.Methods.Count == 10 &&
                pair.Value.Methods.Any(method =>
                    method.Key == "tune-ok" &&
                    method.Value.Name == "tune-ok" &&
                    method.Value.Label == "negotiate connection tuning parameters" &&
                    method.Value.Index == 31 &&
                    method.Value.Documentation.Trim().StartsWith("This method sends the client's connection tuning parameters to t") &&
                    method.Value.Synchronous &&
                    method.Value.HasContent == false &&
                    method.Value.Fields.Count == 3 &&
                    method.Value.Chassis.Count() == 1 &&
                    method.Value.Chassis.Any(chassis => 
                        chassis.Name == ChassisName.Server &&
                        chassis.MustImplement) &&
                    method.Value.Fields.Any(field => 
                        field.Key == "channel-max" &&
                        field.Value.Name == "channel-max" &&
                        field.Value.Domain.Type == "short" &&
                        field.Value.Domain.Name == "short" &&
                        field.Value.Domain.Label == "16-bit integer" &&
                        field.Value.Asserts.Count() == 2 &&
                        field.Value.Asserts.Any(assert =>
                            assert.Check == "le" &&
                            assert.Field.Name == "channel-max" &&
                            assert.Field.Domain.Type == "short" &&
                            assert.Field.Domain.Name == "short" &&
                            assert.Field.Domain.Label == "16-bit integer") &&
                        field.Value.Label == "negotiated maximum channels" &&
                        field.Value.Documentation.Trim().StartsWith("The maximum total number of channels that the client will use per connection.") &&
                        field.Value.Rules.Count() == 1 &&
                        field.Value.Rules.Any(rule => 
                            rule.Documentation.Trim().StartsWith("If the client specifies a channel max that is higher than the value provided") &&
                            rule.Name == "upper-limit"))));

            _protocol.Classes.Should().Contain.One(pair =>
                pair.Key == "basic" &&
                pair.Value.Fields.Count == 14 &&
                pair.Value.Fields.Any(field =>
                    field.Key == "content-type" &&
                    field.Value.Name == "content-type" &&
                    field.Value.Label == "MIME content type" &&
                    field.Value.Domain.Type == "shortstr" &&
                    field.Value.Domain.Name == "shortstr" &&
                    field.Value.Domain.Label == "short string"));
        }
        
        [Fact]
        public void It_should_not_have_any_unresolvable_response_methods()
        {
            var resolvedResponseMethods = new List<Method>();
            foreach (var @class in _protocol.Classes.Values)
            {
                foreach (var method in @class.Methods.Values)
                {
                    resolvedResponseMethods.AddRange(method.Responses.Values.Select(response => response.Method).WhereNotNull());
                }
            }

            resolvedResponseMethods.Count.Should().Be.GreaterThan(0);
        }

        [Fact]
        public void It_should_not_have_any_unresolvable_assert_fields()
        {
            var resolvedAssertFields = new List<Field>();
            foreach (var domain in _protocol.Domains.Values)
            {
                resolvedAssertFields.AddRange(domain.Asserts.Select(assert => assert.Field).WhereNotNull().ToList());
            }

            foreach (var @class in _protocol.Classes.Values)
            {
                foreach (var method in @class.Methods.Values)
                {
                    foreach (var field in method.Fields.Values)
                    {
                        resolvedAssertFields.AddRange(field.Asserts.Select(assert => assert.Field).WhereNotNull().ToList());
                    }
                }
            }

            resolvedAssertFields.Count.Should().Be.GreaterThan(0);
        }
    }
}