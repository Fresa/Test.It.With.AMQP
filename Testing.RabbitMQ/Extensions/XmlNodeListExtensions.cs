using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace Test.It.With.RabbitMQ.Extensions
{
    public static class XmlNodeListExtensions
    {
        [ContractAnnotation("null => true")]
        public static bool IsNullOrEmpty(this XmlNodeList list)
        {
            return list == null || list.Count == 0;
        }

        [ContractAnnotation("null => true")]
        public static bool IsNull(this XmlNodeList list)
        {
            return list == null;
        }

        public static IEnumerable<T> CastOrEmptyList<T>(this XmlNodeList list)
            where T : XmlNode
        {
            if (list.IsNullOrEmpty())
            {
                yield break;
            }

            foreach (T node in list)
            {
                yield return node;
            }
        }
    }
}