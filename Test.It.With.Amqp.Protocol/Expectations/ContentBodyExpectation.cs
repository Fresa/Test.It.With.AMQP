namespace Test.It.With.Amqp.Protocol.Expectations
{
    internal class ContentBodyExpectation : Expectation
    {
        public ContentBodyExpectation(long size)
        {
            Size = size;
        }

        public long Size { get; }
    }
}