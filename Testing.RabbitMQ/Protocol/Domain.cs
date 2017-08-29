using System.Collections.Generic;

namespace Test.It.With.RabbitMQ.Protocol
{
    internal class Domain
    {
        public Domain(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public string Type { get; }

        public string Label { get; set; } = string.Empty;
        public string Documentation { get; set; } = string.Empty;
        public IEnumerable<Rule> Rules { get; set; } = new List<Rule>();
        public IEnumerable<Assert> Asserts { get; set; } = new List<Assert>();
    }
}