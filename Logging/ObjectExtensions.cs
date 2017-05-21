using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Logging
{
    public static class ObjectExtensions
    {
        public static bool IsSerializable(this object obj)
        {
            var type = obj.GetType();

            return Attribute.IsDefined(type, typeof(DataContractAttribute)) ||
                   type.IsSerializable ||
                   obj is IXmlSerializable;
        }
    }
}