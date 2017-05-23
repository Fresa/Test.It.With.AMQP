using System;
using System.Text;
using Newtonsoft.Json;

namespace Test.It.With.RabbitMQ.Tests
{
    public class NewtonsoftSerializer : ISerializer
    {
        private readonly Encoding _defaultEncoding;

        public NewtonsoftSerializer(Encoding defaultEncoding)
        {
            _defaultEncoding = defaultEncoding;
        }

        public T Deserialize<T>(byte[] data)
        {
            return Deserialize<T>(data, _defaultEncoding);
        }

        public T Deserialize<T>(byte[] data, Encoding encoding)
        {
            return JsonConvert.DeserializeObject<T>(encoding.GetString(data));
        }

        public object Deserialize(Type type, byte[] data, Encoding encoding)
        {
            return JsonConvert.DeserializeObject(encoding.GetString(data), type);
        }

        public object Deserialize(Type type, byte[] data)
        {
            return Deserialize(type, data, _defaultEncoding);
        }

        public byte[] Serialize<T>(T data, Encoding encoding)
        {
            return encoding.GetBytes(JsonConvert.SerializeObject(data));
        }

        public byte[] Serialize<T>(T data)
        {
            return Serialize(data, _defaultEncoding);
        }
    }
}