using System;
using System.Text;

namespace Testing.RabbitMQ
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] data);
        T Deserialize<T>(byte[] data, Encoding encoding);
        object Deserialize(Type type, byte[] data, Encoding encoding);
        object Deserialize(Type type, byte[] data);
        byte[] Serialize<T>(T data, Encoding encoding);
        byte[] Serialize<T>(T data);
    }
}