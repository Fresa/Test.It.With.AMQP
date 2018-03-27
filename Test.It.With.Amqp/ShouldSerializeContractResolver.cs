using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp
{
    internal class ShouldSerializeContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            FilterContentMethodProperties(property);

            return property;
        }

        private static void ShouldNotSerialize(JsonProperty property)
        {
            property.ShouldSerialize = o => false;
        }

        private static void Is<T>(JsonProperty property, Action<JsonProperty> action)
        {
            if (property.DeclaringType == typeof(T))
            {
                action(property);
            }
        }

        private static void ImplementsInterface<T>(JsonProperty property, Action<JsonProperty> action)
        {
            if (property.PropertyType.GetInterfaces().Any(type => type == typeof(T)))
            {
                action(property);
            }
        }

        private static void FilterContentMethodProperties(JsonProperty property)
        {
            if (property.DeclaringType.GetInterfaces().Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IContentMethod<>)))
            {
                Is<IContentBody[]>(property, ShouldNotSerialize);
                Is<byte[]>(property, ShouldNotSerialize);
                ImplementsInterface<IContentHeader>(property, ShouldNotSerialize);
            }
        }
    }
}