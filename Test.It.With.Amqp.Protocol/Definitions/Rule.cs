namespace Test.It.With.Amqp.Protocol.Definitions
{
    public class Rule
    {
        public Rule(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string Documentation { get; set; }
        public string ScenarioDocumentation { get; set; }
    }
}