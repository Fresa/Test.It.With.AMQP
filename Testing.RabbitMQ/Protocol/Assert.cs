namespace Test.It.With.RabbitMQ.Protocol
{
    internal class Assert
    {
        public Assert(string check)
        {
            Check = check;
        }

        public string Check { get; }
        public string Value { get; set; }
        public string Method { get; set; }
        public string Field { get; set; }
    }
}