// WARNING! THIS FILE IS AUTO-GENERATED! DO NOT EDIT.

using Validation;
using System.Text.RegularExpressions;

namespace Test.It.With.RabbitMQ
{
	internal struct ClassId 
	{
		internal ClassId(System.Int16 @classId)
		{
		}
	}

	internal struct ConsumerTag 
	{
		internal ConsumerTag(System.String @consumerTag)
		{
		}
	}

	internal struct DeliveryTag 
	{
		internal DeliveryTag(System.Int64 @deliveryTag)
		{
		}
	}

	internal struct ExchangeName 
	{
		internal ExchangeName(System.String @exchangeName)
		{
			Requires.Range(@exchangeName.Length <= 127, nameof(@exchangeName));
			Requires.That(Regex.IsMatch(@exchangeName, "^[a-zA-Z0-9-_.:]*$"), nameof(@exchangeName), "Value must meet the following regex criteria: ^[a-zA-Z0-9-_.:]*$");
		}
	}

	internal struct MethodId 
	{
		internal MethodId(System.Int16 @methodId)
		{
		}
	}

	internal struct NoAck 
	{
		internal NoAck(System.Boolean @noAck)
		{
		}
	}

	internal struct NoLocal 
	{
		internal NoLocal(System.Boolean @noLocal)
		{
		}
	}

	internal struct NoWait 
	{
		internal NoWait(System.Boolean @noWait)
		{
		}
	}

	internal struct Path 
	{
		internal Path(System.String @path)
		{
			Requires.NotNullAllowStructs(@path, nameof(@path));
			Requires.Range(@path.Length <= 127, nameof(@path));
		}
	}

	internal struct PeerProperties 
	{
		internal PeerProperties(System.Data.DataTable @peerProperties)
		{
		}
	}

	internal struct QueueName 
	{
		internal QueueName(System.String @queueName)
		{
			Requires.Range(@queueName.Length <= 127, nameof(@queueName));
			Requires.That(Regex.IsMatch(@queueName, "^[a-zA-Z0-9-_.:]*$"), nameof(@queueName), "Value must meet the following regex criteria: ^[a-zA-Z0-9-_.:]*$");
		}
	}

	internal struct Redelivered 
	{
		internal Redelivered(System.Boolean @redelivered)
		{
		}
	}

	internal struct MessageCount 
	{
		internal MessageCount(System.Int32 @messageCount)
		{
		}
	}

	internal struct ReplyCode 
	{
		internal ReplyCode(System.Int16 @replyCode)
		{
			Requires.NotNullAllowStructs(@replyCode, nameof(@replyCode));
		}
	}

	internal struct ReplyText 
	{
		internal ReplyText(System.String @replyText)
		{
			Requires.NotNullAllowStructs(@replyText, nameof(@replyText));
		}
	}

	internal struct Bit 
	{
		internal Bit(System.Boolean @bit)
		{
		}
	}

	internal struct Octet 
	{
		internal Octet(System.Byte @octet)
		{
		}
	}

	internal struct Short 
	{
		internal Short(System.Int16 @short)
		{
		}
	}

	internal struct Long 
	{
		internal Long(System.Int32 @long)
		{
		}
	}

	internal struct Longlong 
	{
		internal Longlong(System.Int64 @longlong)
		{
		}
	}

	internal struct Shortstr 
	{
		internal Shortstr(System.String @shortstr)
		{
		}
	}

	internal struct Longstr 
	{
		internal Longstr(System.String @longstr)
		{
		}
	}

	internal struct Timestamp 
	{
		internal Timestamp(System.DateTime @timestamp)
		{
		}
	}

	internal struct Table 
	{
		internal Table(System.Data.DataTable @table)
		{
		}
	}
}