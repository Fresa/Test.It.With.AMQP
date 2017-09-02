namespace Test.It.With.RabbitMQ.Protocol
{
    public class Constant
    {
        public Constant(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public int Value { get; }
        public string Documentation { get; set; }
        public string Class { get; set; }
    }
}