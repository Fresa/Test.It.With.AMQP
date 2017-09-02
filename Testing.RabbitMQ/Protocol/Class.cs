using System.Collections.Generic;

namespace Test.It.With.RabbitMQ.Protocol
{
    public class Class
    {
        public string Name { get; }
        public string Handler { get; }
        public int Index { get; }
        public IReadOnlyDictionary<string, Method> Methods { get; }
        public string Label { get; set; }
        public string Documentation { get; set; }
        public string GrammarDocumentation { get; set; }
        public IEnumerable<Chassis> Chassis { get; set; }

        public Class(string name, string handler, int index, IReadOnlyDictionary<string, Method> methods)
        {
            Name = name;
            Handler = handler;
            Index = index;
            Methods = methods;
        }
    }
}