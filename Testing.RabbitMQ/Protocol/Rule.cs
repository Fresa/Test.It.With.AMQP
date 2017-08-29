namespace Test.It.With.RabbitMQ.Protocol
{
    internal class Rule
    {
        public Rule(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public string Documentation { get; set; }
    }
}