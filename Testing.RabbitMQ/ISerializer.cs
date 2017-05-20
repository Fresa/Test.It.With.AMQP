using System;

namespace Testing.RabbitMQ
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] data);
        object Deserialize(Type type, byte[] data);
        byte[] Serialize<T>(T data);
        byte[] Serialize(Type type, object data);
    }
}