namespace Test.It.With.Amqp.Protocol
{
    public interface IContentBody
    {
        byte[] Payload { get; }
        
        void ReadFrom(AmqpReader reader);
        void WriteTo(AmqpWriter writer);
    }

    //public class ContentBody : IContentBody
    //{
    //    private readonly long _size;

    //    public ContentBody(long size)
    //    {
    //        _size = size;
    //    }

    //    public byte[] Payload { get; private set; }

    //    public void ReadFrom(AmqpReader reader)
    //    {
    //        Payload = reader.ReadBytes((int) _size);
    //        if (reader.ReadByte() != Constants.)

    //    }

    //    public void WriteTo(AmqpWriter writer)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}