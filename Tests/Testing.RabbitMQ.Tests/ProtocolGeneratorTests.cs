using System;
using System.IO;
using System.Xml;
using Test.It.With.RabbitMQ.Protocol;
using Test.It.With.XUnit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class ProtocolGeneratorTests : XUnit2Specification
    {
//        private ProtocolGenerator _generator;

        protected override void Given()
        {
            var path = Path.Combine(Environment.CurrentDirectory, @"Resources\amqp0-9-1\amqp0-9-1.xml");
            var definition = new XmlDocument();
            definition.Load(path);

  //          _generator = new ProtocolGenerator(new Protocol.Protocol(definition));
        }

        protected override void When()
        {
            //var generatedClass = _generator.TransformText();
            //File.Create(Path.Combine(Environment.CurrentDirectory))
        }
    }
}