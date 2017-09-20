using System.Collections.Generic;
using System.Linq;

namespace Test.It.With.Amqp.Protocol.Definitions
{
    public class Method
    {
        public string Name { get; }
        public int Index { get; }

        public bool Synchronous { get; set; }
        public IReadOnlyDictionary<string, Field> Fields { get; set; } = new Dictionary<string, Field>();
        public IReadOnlyDictionary<string, Response> Responses { get; set; } = new Dictionary<string, Response>();
        public string Label { get; set; }
        public string Documentation { get; set; }
        public IEnumerable<Rule> Rules { get; set; } = Enumerable.Empty<Rule>();
        public IEnumerable<Chassis> Chassis { get; set; } = Enumerable.Empty<Chassis>();
        public bool HasContent { get; set; }

        public Method(string name, int index)
        {
            Name = name;
            Index = index;
        }
    }
}