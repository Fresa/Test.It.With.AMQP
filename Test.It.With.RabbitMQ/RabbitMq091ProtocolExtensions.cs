using System;
using System.Collections.Generic;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.RabbitMQ._091
{
    internal class RabbitMq091ProtocolExtensions
    {
        public bool TryGetMethod(IAmqpReader reader, out IMethod method)
        {
            method = default;

            var classId = reader.ReadShortUnsignedInteger();
            if (_classRegister.TryGetValue(classId, out var methodRegister) == false)
            {
                return false;
            }

            var methodId = reader.ReadShortUnsignedInteger();
            if (methodRegister.TryGetValue(methodId, out var methodFactory) == false)
            {
                return false;
            }

            method = methodFactory();
            return true;
        }

        private readonly Dictionary<int, Dictionary<int, Func<IMethod>>> _classRegister = new Dictionary<int, Dictionary<int, Func<IMethod>>>
        {
            { Confirm.ClassId, new Dictionary<int, Func<IMethod>> {
                { Confirm.Select.MethodId, () => new Confirm.Select() },
                { Confirm.SelectOk.MethodId, () => new Confirm.SelectOk() }}}
        };
    }

    public class Confirm
    {
        public const int ClassId = 85;

        public class Select : IRespond<SelectOk>, IClientMethod
        {
            internal const int MethodId = 10;
            public int ProtocolClassId { get; } = ClassId;
            public int ProtocolMethodId { get; } = MethodId;

            public bool Nowait { get; set; }

            public Type[] Responses()
            {
                return new[] { typeof(SelectOk) };
            }

            public bool SentOnValidChannel(int channel)
            {
                return channel > 0;
            }

            public void ReadFrom(IAmqpReader reader)
            {
                Nowait = reader.ReadBit();
            }

            public void WriteTo(IAmqpWriter writer)
            {
                writer.WriteBit(Nowait);
            }

            public SelectOk Respond(SelectOk method)
            {
                return method;
            }
        }

        public class SelectOk : IServerMethod, INonContentMethod
        {
            internal const int MethodId = 11;
            public int ProtocolClassId { get; } = ClassId;
            public int ProtocolMethodId { get; } = MethodId;

            public bool SentOnValidChannel(int channel)
            {
                return channel > 0;
            }

            public void ReadFrom(IAmqpReader reader)
            {
            }

            public void WriteTo(IAmqpWriter writer)
            {
                writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
                writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);
            }

            public Type[] Responses()
            {
                return new Type[0];
            }
        }
    }

    public class Basic
    {
        public class Ack : Test.It.With.Amqp.Protocol._091.Basic.Ack, IServerMethod { }
    }
}