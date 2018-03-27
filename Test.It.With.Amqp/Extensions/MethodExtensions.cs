using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Test.It.With.Amqp.Extensions
{
    internal static class MethodExtensions
    {
        public static string Serialize<T>(this T method) where T : class
        {
            return JsonConvert.SerializeObject(method, new JsonSerializerSettings
            {
                ContractResolver = new ShouldSerializeContractResolver()
            });
        }
    }
}