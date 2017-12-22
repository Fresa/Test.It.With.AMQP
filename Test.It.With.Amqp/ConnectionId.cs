using System;

namespace Test.It.With.Amqp
{
    public struct ConnectionId
    {
        public ConnectionId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public override bool Equals(object obj)
        {
            return obj is ConnectionId connectionId && this == connectionId;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(ConnectionId x, ConnectionId y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(ConnectionId x, ConnectionId y)
        {
            return !(x == y);
        }
    }
}