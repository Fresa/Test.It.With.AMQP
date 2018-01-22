// WARNING! THIS FILE IS AUTO-GENERATED! DO NOT EDIT.

using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Validation;
using Log.It;

namespace Test.It.With.Amqp.Protocol._091
{
	internal class Amq091Protocol : IProtocol
	{
		public IVersion Version { get; } = new ProtocolVersion(); 

		public IProtocolHeader GetProtocolHeader(IAmqpReader reader)
		{
			var protocolHeader = new ProtocolHeader();
			protocolHeader.ReadFrom(reader);
			return protocolHeader;
		}

		public IMethod GetMethod(IAmqpReader reader)
		{
			var classId = reader.ReadShortUnsignedInteger();

			if (_methodFactory.TryGetValue(classId, out Dictionary<int, Func<IMethod>> methodRegister) == false)
			{
				throw new FrameErrorException($"There is no class with id {classId} defined.");
			}

			var methodId = reader.ReadShortUnsignedInteger();

			if (methodRegister.TryGetValue(methodId, out Func<IMethod> factory) == false)
			{
				throw new FrameErrorException($"There is no method defined with id {methodId} in class with id {classId}.");
			}

			var method = factory();
			method.ReadFrom(reader);
			return method;
		}

		public IContentHeader GetContentHeader(IAmqpReader reader)
		{
			var classId = reader.ReadShortUnsignedInteger();

			var weight = reader.ReadShortInteger();
			if (weight != 0)
			{
				throw new FrameErrorException("Expected weight = 0");
			}

			if (_contentHeaderFactory.TryGetValue(classId, out Func<IContentHeader> factory) == false)
			{
				throw new FrameErrorException($"There is no content header defined for class with id {classId}.");
			}

			var contentHeader = factory();
			contentHeader.ReadFrom(reader);
			return contentHeader;
		}

		public IContentBody GetContentBody(IAmqpReader reader)
		{
			var contentBody = new ContentBody();
			contentBody.ReadFrom(reader);
			return contentBody;
		}

		public IHeartbeat GetHeartbeat(IAmqpReader reader)
		{
			var heartbeat = new Heartbeat();
			heartbeat.ReadFrom(reader);
			return heartbeat;
		}

		private readonly Dictionary<int, Dictionary<int, Func<IMethod>>> _methodFactory = new Dictionary<int, Dictionary<int, Func<IMethod>>>
		{
			{ 10, new Dictionary<int, Func<IMethod>> { 
				{ 10, () => new Connection.Start() },
				{ 11, () => new Connection.StartOk() },
				{ 20, () => new Connection.Secure() },
				{ 21, () => new Connection.SecureOk() },
				{ 30, () => new Connection.Tune() },
				{ 31, () => new Connection.TuneOk() },
				{ 40, () => new Connection.Open() },
				{ 41, () => new Connection.OpenOk() },
				{ 50, () => new Connection.Close() },
				{ 51, () => new Connection.CloseOk() }}},
			{ 20, new Dictionary<int, Func<IMethod>> { 
				{ 10, () => new Channel.Open() },
				{ 11, () => new Channel.OpenOk() },
				{ 20, () => new Channel.Flow() },
				{ 21, () => new Channel.FlowOk() },
				{ 40, () => new Channel.Close() },
				{ 41, () => new Channel.CloseOk() }}},
			{ 40, new Dictionary<int, Func<IMethod>> { 
				{ 10, () => new Exchange.Declare() },
				{ 11, () => new Exchange.DeclareOk() },
				{ 20, () => new Exchange.Delete() },
				{ 21, () => new Exchange.DeleteOk() }}},
			{ 50, new Dictionary<int, Func<IMethod>> { 
				{ 10, () => new Queue.Declare() },
				{ 11, () => new Queue.DeclareOk() },
				{ 20, () => new Queue.Bind() },
				{ 21, () => new Queue.BindOk() },
				{ 50, () => new Queue.Unbind() },
				{ 51, () => new Queue.UnbindOk() },
				{ 30, () => new Queue.Purge() },
				{ 31, () => new Queue.PurgeOk() },
				{ 40, () => new Queue.Delete() },
				{ 41, () => new Queue.DeleteOk() }}},
			{ 60, new Dictionary<int, Func<IMethod>> { 
				{ 10, () => new Basic.Qos() },
				{ 11, () => new Basic.QosOk() },
				{ 20, () => new Basic.Consume() },
				{ 21, () => new Basic.ConsumeOk() },
				{ 30, () => new Basic.Cancel() },
				{ 31, () => new Basic.CancelOk() },
				{ 40, () => new Basic.Publish() },
				{ 50, () => new Basic.Return() },
				{ 60, () => new Basic.Deliver() },
				{ 70, () => new Basic.Get() },
				{ 71, () => new Basic.GetOk() },
				{ 72, () => new Basic.GetEmpty() },
				{ 80, () => new Basic.Ack() },
				{ 90, () => new Basic.Reject() },
				{ 100, () => new Basic.RecoverAsync() },
				{ 110, () => new Basic.Recover() },
				{ 111, () => new Basic.RecoverOk() }}},
			{ 90, new Dictionary<int, Func<IMethod>> { 
				{ 10, () => new Tx.Select() },
				{ 11, () => new Tx.SelectOk() },
				{ 20, () => new Tx.Commit() },
				{ 21, () => new Tx.CommitOk() },
				{ 30, () => new Tx.Rollback() },
				{ 31, () => new Tx.RollbackOk() }}}
		};

		private readonly Dictionary<int, Func<IContentHeader>> _contentHeaderFactory = new Dictionary<int, Func<IContentHeader>>
		{
			{ 60, () => new Basic.ContentHeader() }
		};

		private class ProtocolVersion : IVersion
		{
			public int Major { get; } = 0;
			public int Minor { get; } = 9;
			public int Revision { get; } = 1;
		}
	}

	internal class Constants
	{
		internal const int FrameMethod = 1;

		internal const int FrameHeader = 2;

		internal const int FrameBody = 3;

		internal const int FrameHeartbeat = 8;

		internal const int FrameMinSize = 4096;

		internal const int FrameEnd = 206;

		/// <summary>
		/// Indicates that the method completed successfully. This reply code is
		/// reserved for future use - the current protocol design does not use positive
		/// confirmation and reply codes are sent only in case of an error.
		/// </summary>
		internal const int ReplySuccess = 200;

		/// <summary>
		/// The client attempted to transfer content larger than the server could accept
		/// at the present time. The client may retry at a later time.
		/// </summary>
		internal const int ContentTooLarge = 311;

		/// <summary>
		/// When the exchange cannot deliver to a consumer when the immediate flag is
		/// set. As a result of pending data on the queue or the absence of any
		/// consumers of the queue.
		/// </summary>
		internal const int NoConsumers = 313;

		/// <summary>
		/// An operator intervened to close the connection for some reason. The client
		/// may retry at some later date.
		/// </summary>
		internal const int ConnectionForced = 320;

		/// <summary>
		/// The client tried to work with an unknown virtual host.
		/// </summary>
		internal const int InvalidPath = 402;

		/// <summary>
		/// The client attempted to work with a server entity to which it has no
		/// access due to security settings.
		/// </summary>
		internal const int AccessRefused = 403;

		/// <summary>
		/// The client attempted to work with a server entity that does not exist.
		/// </summary>
		internal const int NotFound = 404;

		/// <summary>
		/// The client attempted to work with a server entity to which it has no
		/// access because another client is working with it.
		/// </summary>
		internal const int ResourceLocked = 405;

		/// <summary>
		/// The client requested a method that was not allowed because some precondition
		/// failed.
		/// </summary>
		internal const int PreconditionFailed = 406;

		/// <summary>
		/// The sender sent a malformed frame that the recipient could not decode.
		/// This strongly implies a programming error in the sending peer.
		/// </summary>
		internal const int FrameError = 501;

		/// <summary>
		/// The sender sent a frame that contained illegal values for one or more
		/// fields. This strongly implies a programming error in the sending peer.
		/// </summary>
		internal const int SyntaxError = 502;

		/// <summary>
		/// The client sent an invalid sequence of frames, attempting to perform an
		/// operation that was considered invalid by the server. This usually implies
		/// a programming error in the client.
		/// </summary>
		internal const int CommandInvalid = 503;

		/// <summary>
		/// The client attempted to work with a channel that had not been correctly
		/// opened. This most likely indicates a fault in the client layer.
		/// </summary>
		internal const int ChannelError = 504;

		/// <summary>
		/// The peer sent a frame that was not expected, usually in the context of
		/// a content header and body.  This strongly indicates a fault in the peer's
		/// content processing.
		/// </summary>
		internal const int UnexpectedFrame = 505;

		/// <summary>
		/// The server could not complete the method because it lacked sufficient
		/// resources. This may be due to the client creating too many of some type
		/// of entity.
		/// </summary>
		internal const int ResourceError = 506;

		/// <summary>
		/// The client tried to work with some entity in a manner that is prohibited
		/// by the server, due to security settings or by some other criteria.
		/// </summary>
		internal const int NotAllowed = 530;

		/// <summary>
		/// The client tried to use functionality that is not implemented in the
		/// server.
		/// </summary>
		internal const int NotImplemented = 540;

		/// <summary>
		/// The server could not complete the method because of an internal error.
		/// The server may require intervention by an operator in order to resume
		/// normal operations.
		/// </summary>
		internal const int InternalError = 541;
	}

	public abstract class AmqpException : Exception 
	{
		protected AmqpException()
		{

		}

		protected AmqpException(string message) : base(message)
		{

		}

		public abstract int Code { get; }
	}

	public abstract class SoftErrorException : AmqpException 
	{
		protected SoftErrorException()
		{

		}

		protected SoftErrorException(string message) : base(message)
		{

		}
	}

	public abstract class HardErrorException : AmqpException 
	{
		protected HardErrorException()
		{

		}

		protected HardErrorException(string message) : base(message)
		{

		}
	}

	public class ContentTooLargeException : SoftErrorException
	{
		public ContentTooLargeException()
		{

		}

		public ContentTooLargeException(string message) : base(message)
		{

		}

		public override int Code { get; } = 311;
	}

	public class NoConsumersException : SoftErrorException
	{
		public NoConsumersException()
		{

		}

		public NoConsumersException(string message) : base(message)
		{

		}

		public override int Code { get; } = 313;
	}

	public class ConnectionForcedException : HardErrorException
	{
		public ConnectionForcedException()
		{

		}

		public ConnectionForcedException(string message) : base(message)
		{

		}

		public override int Code { get; } = 320;
	}

	public class InvalidPathException : HardErrorException
	{
		public InvalidPathException()
		{

		}

		public InvalidPathException(string message) : base(message)
		{

		}

		public override int Code { get; } = 402;
	}

	public class AccessRefusedException : SoftErrorException
	{
		public AccessRefusedException()
		{

		}

		public AccessRefusedException(string message) : base(message)
		{

		}

		public override int Code { get; } = 403;
	}

	public class NotFoundException : SoftErrorException
	{
		public NotFoundException()
		{

		}

		public NotFoundException(string message) : base(message)
		{

		}

		public override int Code { get; } = 404;
	}

	public class ResourceLockedException : SoftErrorException
	{
		public ResourceLockedException()
		{

		}

		public ResourceLockedException(string message) : base(message)
		{

		}

		public override int Code { get; } = 405;
	}

	public class PreconditionFailedException : SoftErrorException
	{
		public PreconditionFailedException()
		{

		}

		public PreconditionFailedException(string message) : base(message)
		{

		}

		public override int Code { get; } = 406;
	}

	public class FrameErrorException : HardErrorException
	{
		public FrameErrorException()
		{

		}

		public FrameErrorException(string message) : base(message)
		{

		}

		public override int Code { get; } = 501;
	}

	public class SyntaxErrorException : HardErrorException
	{
		public SyntaxErrorException()
		{

		}

		public SyntaxErrorException(string message) : base(message)
		{

		}

		public override int Code { get; } = 502;
	}

	public class CommandInvalidException : HardErrorException
	{
		public CommandInvalidException()
		{

		}

		public CommandInvalidException(string message) : base(message)
		{

		}

		public override int Code { get; } = 503;
	}

	public class ChannelErrorException : HardErrorException
	{
		public ChannelErrorException()
		{

		}

		public ChannelErrorException(string message) : base(message)
		{

		}

		public override int Code { get; } = 504;
	}

	public class UnexpectedFrameException : HardErrorException
	{
		public UnexpectedFrameException()
		{

		}

		public UnexpectedFrameException(string message) : base(message)
		{

		}

		public override int Code { get; } = 505;
	}

	public class ResourceErrorException : HardErrorException
	{
		public ResourceErrorException()
		{

		}

		public ResourceErrorException(string message) : base(message)
		{

		}

		public override int Code { get; } = 506;
	}

	public class NotAllowedException : HardErrorException
	{
		public NotAllowedException()
		{

		}

		public NotAllowedException(string message) : base(message)
		{

		}

		public override int Code { get; } = 530;
	}

	public class NotImplementedException : HardErrorException
	{
		public NotImplementedException()
		{

		}

		public NotImplementedException(string message) : base(message)
		{

		}

		public override int Code { get; } = 540;
	}

	public class InternalErrorException : HardErrorException
	{
		public InternalErrorException()
		{

		}

		public InternalErrorException(string message) : base(message)
		{

		}

		public override int Code { get; } = 541;
	}

	public struct ClassId 
	{
		public System.Int16 Value { get; }

		public ClassId(System.Int16 value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is ClassId comparingClassId && this == comparingClassId;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (ClassId x, ClassId y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (ClassId x, ClassId y)
		{
			return !(x == y);
		}

		public static ClassId From(System.Int16 value)
		{
			return new ClassId(value);
		}
	}

	/// <summary>
	/// Identifier for the consumer, valid within the current channel.
	/// </summary>
	public struct ConsumerTag 
	{
		public System.String Value { get; }

		public ConsumerTag(System.String value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is ConsumerTag comparingConsumerTag && this == comparingConsumerTag;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (ConsumerTag x, ConsumerTag y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (ConsumerTag x, ConsumerTag y)
		{
			return !(x == y);
		}

		public static ConsumerTag From(System.String value)
		{
			return new ConsumerTag(value);
		}
	}

	/// <summary>
	/// The server-assigned and channel-specific delivery tag
	/// </summary>
	public struct DeliveryTag 
	{
		public System.Int64 Value { get; }

		public DeliveryTag(System.Int64 value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is DeliveryTag comparingDeliveryTag && this == comparingDeliveryTag;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (DeliveryTag x, DeliveryTag y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (DeliveryTag x, DeliveryTag y)
		{
			return !(x == y);
		}

		public static DeliveryTag From(System.Int64 value)
		{
			return new DeliveryTag(value);
		}
	}

	/// <summary>
	/// The exchange name is a client-selected string that identifies the exchange for
	/// publish methods.
	/// </summary>
	public struct ExchangeName 
	{
		public System.String Value { get; }

		public ExchangeName(System.String value)
		{
			Requires.Range(value.Length <= 127, nameof(value));
			Requires.That(Regex.IsMatch(value, "^[a-zA-Z0-9-_.:]*$"), nameof(value), "Value must meet the following regex criteria: ^[a-zA-Z0-9-_.:]*$");
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is ExchangeName comparingExchangeName && this == comparingExchangeName;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (ExchangeName x, ExchangeName y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (ExchangeName x, ExchangeName y)
		{
			return !(x == y);
		}

		public static ExchangeName From(System.String value)
		{
			return new ExchangeName(value);
		}
	}

	public struct MethodId 
	{
		public System.Int16 Value { get; }

		public MethodId(System.Int16 value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is MethodId comparingMethodId && this == comparingMethodId;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (MethodId x, MethodId y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (MethodId x, MethodId y)
		{
			return !(x == y);
		}

		public static MethodId From(System.Int16 value)
		{
			return new MethodId(value);
		}
	}

	/// <summary>
	/// If this field is set the server does not expect acknowledgements for
	/// messages. That is, when a message is delivered to the client the server
	/// assumes the delivery will succeed and immediately dequeues it. This
	/// functionality may increase performance but at the cost of reliability.
	/// Messages can get lost if a client dies before they are delivered to the
	/// application.
	/// </summary>
	public struct NoAck 
	{
		public System.Boolean Value { get; }

		public NoAck(System.Boolean value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is NoAck comparingNoAck && this == comparingNoAck;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (NoAck x, NoAck y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (NoAck x, NoAck y)
		{
			return !(x == y);
		}

		public static NoAck From(System.Boolean value)
		{
			return new NoAck(value);
		}
	}

	/// <summary>
	/// If the no-local field is set the server will not send messages to the connection that
	/// published them.
	/// </summary>
	public struct NoLocal 
	{
		public System.Boolean Value { get; }

		public NoLocal(System.Boolean value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is NoLocal comparingNoLocal && this == comparingNoLocal;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (NoLocal x, NoLocal y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (NoLocal x, NoLocal y)
		{
			return !(x == y);
		}

		public static NoLocal From(System.Boolean value)
		{
			return new NoLocal(value);
		}
	}

	/// <summary>
	/// If set, the server will not respond to the method. The client should not wait
	/// for a reply method. If the server could not complete the method it will raise a
	/// channel or connection exception.
	/// </summary>
	public struct NoWait 
	{
		public System.Boolean Value { get; }

		public NoWait(System.Boolean value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is NoWait comparingNoWait && this == comparingNoWait;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (NoWait x, NoWait y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (NoWait x, NoWait y)
		{
			return !(x == y);
		}

		public static NoWait From(System.Boolean value)
		{
			return new NoWait(value);
		}
	}

	/// <summary>
	/// Unconstrained.
	/// </summary>
	public struct Path 
	{
		public System.String Value { get; }

		public Path(System.String value)
		{
			Requires.NotNullAllowStructs(value, nameof(value));
			Requires.Range(value.Length <= 127, nameof(value));
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Path comparingPath && this == comparingPath;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Path x, Path y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Path x, Path y)
		{
			return !(x == y);
		}

		public static Path From(System.String value)
		{
			return new Path(value);
		}
	}

	/// <summary>
	/// This table provides a set of peer properties, used for identification, debugging,
	/// and general information.
	/// </summary>
	public struct PeerProperties 
	{
		public System.Collections.Generic.IDictionary<System.String, System.Object> Value { get; }

		public PeerProperties(System.Collections.Generic.IDictionary<System.String, System.Object> value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is PeerProperties comparingPeerProperties && this == comparingPeerProperties;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (PeerProperties x, PeerProperties y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (PeerProperties x, PeerProperties y)
		{
			return !(x == y);
		}

		public static PeerProperties From(System.Collections.Generic.IDictionary<System.String, System.Object> value)
		{
			return new PeerProperties(value);
		}
	}

	/// <summary>
	/// The queue name identifies the queue within the vhost.  In methods where the queue
	/// name may be blank, and that has no specific significance, this refers to the
	/// 'current' queue for the channel, meaning the last queue that the client declared
	/// on the channel.  If the client did not declare a queue, and the method needs a
	/// queue name, this will result in a 502 (syntax error) channel exception.
	/// </summary>
	public struct QueueName 
	{
		public System.String Value { get; }

		public QueueName(System.String value)
		{
			Requires.Range(value.Length <= 127, nameof(value));
			Requires.That(Regex.IsMatch(value, "^[a-zA-Z0-9-_.:]*$"), nameof(value), "Value must meet the following regex criteria: ^[a-zA-Z0-9-_.:]*$");
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is QueueName comparingQueueName && this == comparingQueueName;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (QueueName x, QueueName y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (QueueName x, QueueName y)
		{
			return !(x == y);
		}

		public static QueueName From(System.String value)
		{
			return new QueueName(value);
		}
	}

	/// <summary>
	/// This indicates that the message has been previously delivered to this or
	/// another client.
	/// </summary>
	public struct Redelivered 
	{
		public System.Boolean Value { get; }

		public Redelivered(System.Boolean value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Redelivered comparingRedelivered && this == comparingRedelivered;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Redelivered x, Redelivered y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Redelivered x, Redelivered y)
		{
			return !(x == y);
		}

		public static Redelivered From(System.Boolean value)
		{
			return new Redelivered(value);
		}
	}

	/// <summary>
	/// The number of messages in the queue, which will be zero for newly-declared
	/// queues. This is the number of messages present in the queue, and committed
	/// if the channel on which they were published is transacted, that are not
	/// waiting acknowledgement.
	/// </summary>
	public struct MessageCount 
	{
		public System.Int32 Value { get; }

		public MessageCount(System.Int32 value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is MessageCount comparingMessageCount && this == comparingMessageCount;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (MessageCount x, MessageCount y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (MessageCount x, MessageCount y)
		{
			return !(x == y);
		}

		public static MessageCount From(System.Int32 value)
		{
			return new MessageCount(value);
		}
	}

	/// <summary>
	/// The reply code. The AMQ reply codes are defined as constants at the start
	/// of this formal specification.
	/// </summary>
	public struct ReplyCode 
	{
		public System.Int16 Value { get; }

		public ReplyCode(System.Int16 value)
		{
			Requires.NotNullAllowStructs(value, nameof(value));
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is ReplyCode comparingReplyCode && this == comparingReplyCode;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (ReplyCode x, ReplyCode y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (ReplyCode x, ReplyCode y)
		{
			return !(x == y);
		}

		public static ReplyCode From(System.Int16 value)
		{
			return new ReplyCode(value);
		}
	}

	/// <summary>
	/// The localised reply text. This text can be logged as an aid to resolving
	/// issues.
	/// </summary>
	public struct ReplyText 
	{
		public System.String Value { get; }

		public ReplyText(System.String value)
		{
			Requires.NotNullAllowStructs(value, nameof(value));
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is ReplyText comparingReplyText && this == comparingReplyText;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (ReplyText x, ReplyText y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (ReplyText x, ReplyText y)
		{
			return !(x == y);
		}

		public static ReplyText From(System.String value)
		{
			return new ReplyText(value);
		}
	}

	public struct Bit 
	{
		public System.Boolean Value { get; }

		public Bit(System.Boolean value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Bit comparingBit && this == comparingBit;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Bit x, Bit y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Bit x, Bit y)
		{
			return !(x == y);
		}

		public static Bit From(System.Boolean value)
		{
			return new Bit(value);
		}
	}

	public struct Octet 
	{
		public System.Byte Value { get; }

		public Octet(System.Byte value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Octet comparingOctet && this == comparingOctet;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Octet x, Octet y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Octet x, Octet y)
		{
			return !(x == y);
		}

		public static Octet From(System.Byte value)
		{
			return new Octet(value);
		}
	}

	public struct Short 
	{
		public System.Int16 Value { get; }

		public Short(System.Int16 value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Short comparingShort && this == comparingShort;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Short x, Short y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Short x, Short y)
		{
			return !(x == y);
		}

		public static Short From(System.Int16 value)
		{
			return new Short(value);
		}
	}

	public struct Long 
	{
		public System.Int32 Value { get; }

		public Long(System.Int32 value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Long comparingLong && this == comparingLong;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Long x, Long y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Long x, Long y)
		{
			return !(x == y);
		}

		public static Long From(System.Int32 value)
		{
			return new Long(value);
		}
	}

	public struct Longlong 
	{
		public System.Int64 Value { get; }

		public Longlong(System.Int64 value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Longlong comparingLonglong && this == comparingLonglong;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Longlong x, Longlong y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Longlong x, Longlong y)
		{
			return !(x == y);
		}

		public static Longlong From(System.Int64 value)
		{
			return new Longlong(value);
		}
	}

	public struct Shortstr 
	{
		public System.String Value { get; }

		public Shortstr(System.String value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Shortstr comparingShortstr && this == comparingShortstr;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Shortstr x, Shortstr y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Shortstr x, Shortstr y)
		{
			return !(x == y);
		}

		public static Shortstr From(System.String value)
		{
			return new Shortstr(value);
		}
	}

	public struct Longstr 
	{
		public System.Byte[] Value { get; }

		public Longstr(System.Byte[] value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Longstr comparingLongstr && this == comparingLongstr;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Longstr x, Longstr y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Longstr x, Longstr y)
		{
			return !(x == y);
		}

		public static Longstr From(System.Byte[] value)
		{
			return new Longstr(value);
		}
	}

	public struct Timestamp 
	{
		public System.DateTime Value { get; }

		public Timestamp(System.DateTime value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Timestamp comparingTimestamp && this == comparingTimestamp;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Timestamp x, Timestamp y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Timestamp x, Timestamp y)
		{
			return !(x == y);
		}

		public static Timestamp From(System.DateTime value)
		{
			return new Timestamp(value);
		}
	}

	public struct Table 
	{
		public System.Collections.Generic.IDictionary<System.String, System.Object> Value { get; }

		public Table(System.Collections.Generic.IDictionary<System.String, System.Object> value)
		{
			Value = value;
		}

		public override bool Equals(object obj) 
		{
			return obj is Table comparingTable && this == comparingTable;
		}

		public override int GetHashCode() 
		{
			return Value.GetHashCode();
		}

		public override string ToString() 
		{
			return Value.ToString();
		}

		public static bool operator == (Table x, Table y)
		{
			return x.Value == y.Value;
		}

		public static bool operator != (Table x, Table y)
		{
			return !(x == y);
		}

		public static Table From(System.Collections.Generic.IDictionary<System.String, System.Object> value)
		{
			return new Table(value);
		}
	}

	/// <summary>
	/// The connection class provides methods for a client to establish a network connection to
	/// a server, and for both peers to operate the connection thereafter.
	/// </summary>
	/// <example>
	/// 
	///       connection          = open-connection *use-connection close-connection
	///       open-connection     = C:protocol-header
	///                             S:START C:START-OK
	///                             *challenge
	///                             S:TUNE C:TUNE-OK
	///                             C:OPEN S:OPEN-OK
	///       challenge           = S:SECURE C:SECURE-OK
	///       use-connection      = *channel
	///       close-connection    = C:CLOSE S:CLOSE-OK
	///                           / S:CLOSE C:CLOSE-OK
	/// </example>
	public class Connection
	{
		/// <summary>
		/// This method starts the connection negotiation process by telling the client the
		/// protocol version that the server proposes, along with a list of security mechanisms
		/// which the client can use for authentication.
		/// </summary>
		public class Start : IMethod, INonContentMethod, IRespond<StartOk>, IServerMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 10;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public StartOk Respond(StartOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(StartOk) };
			}

			private Octet _versionMajor;
			/// <summary>
			/// The major version number can take any value from 0 to 99 as defined in the
			/// AMQP specification.
			/// </summary>
			public Octet VersionMajor
			{
				get => _versionMajor;
				set
				{
					_versionMajor = value;
				}
			}

			private Octet _versionMinor;
			/// <summary>
			/// The minor version number can take any value from 0 to 99 as defined in the
			/// AMQP specification.
			/// </summary>
			public Octet VersionMinor
			{
				get => _versionMinor;
				set
				{
					_versionMinor = value;
				}
			}

			private PeerProperties _serverProperties;
			public PeerProperties ServerProperties
			{
				get => _serverProperties;
				set
				{
					_serverProperties = value;
				}
			}

			private Longstr _mechanisms;
			/// <summary>
			/// A list of the security mechanisms that the server supports, delimited by spaces.
			/// </summary>
			public Longstr Mechanisms
			{
				get => _mechanisms;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_mechanisms = value;
				}
			}

			private Longstr _locales;
			/// <summary>
			/// A list of the message locales that the server supports, delimited by spaces. The
			/// locale defines the language in which the server will send reply texts.
			/// </summary>
			public Longstr Locales
			{
				get => _locales;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_locales = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_versionMajor = new Octet(reader.ReadByte());
				_versionMinor = new Octet(reader.ReadByte());
				_serverProperties = new PeerProperties(reader.ReadTable());
				_mechanisms = new Longstr(reader.ReadLongString());
				_locales = new Longstr(reader.ReadLongString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteByte(_versionMajor.Value);
				writer.WriteByte(_versionMinor.Value);
				writer.WriteTable(_serverProperties.Value);
				writer.WriteLongString(_mechanisms.Value);
				writer.WriteLongString(_locales.Value);
			}
		}

		/// <summary>
		/// This method selects a SASL security mechanism.
		/// </summary>
		public class StartOk : IMethod, INonContentMethod, IClientMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 11;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private PeerProperties _clientProperties;
			public PeerProperties ClientProperties
			{
				get => _clientProperties;
				set
				{
					_clientProperties = value;
				}
			}

			private Shortstr _mechanism;
			/// <summary>
			/// A single security mechanisms selected by the client, which must be one of those
			/// specified by the server.
			/// </summary>
			public Shortstr Mechanism
			{
				get => _mechanism;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_mechanism = value;
				}
			}

			private Longstr _response;
			/// <summary>
			/// A block of opaque data passed to the security mechanism. The contents of this
			/// data are defined by the SASL security mechanism.
			/// </summary>
			public Longstr Response
			{
				get => _response;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_response = value;
				}
			}

			private Shortstr _locale;
			/// <summary>
			/// A single message locale selected by the client, which must be one of those
			/// specified by the server.
			/// </summary>
			public Shortstr Locale
			{
				get => _locale;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_locale = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_clientProperties = new PeerProperties(reader.ReadTable());
				_mechanism = new Shortstr(reader.ReadShortString());
				_response = new Longstr(reader.ReadLongString());
				_locale = new Shortstr(reader.ReadShortString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteTable(_clientProperties.Value);
				writer.WriteShortString(_mechanism.Value);
				writer.WriteLongString(_response.Value);
				writer.WriteShortString(_locale.Value);
			}
		}

		/// <summary>
		/// The SASL protocol works by exchanging challenges and responses until both peers have
		/// received sufficient information to authenticate each other. This method challenges
		/// the client to provide more information.
		/// </summary>
		public class Secure : IMethod, INonContentMethod, IRespond<SecureOk>, IServerMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 20;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public SecureOk Respond(SecureOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(SecureOk) };
			}

			private Longstr _challenge;
			/// <summary>
			/// Challenge information, a block of opaque binary data passed to the security
			/// mechanism.
			/// </summary>
			public Longstr Challenge
			{
				get => _challenge;
				set
				{
					_challenge = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_challenge = new Longstr(reader.ReadLongString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongString(_challenge.Value);
			}
		}

		/// <summary>
		/// This method attempts to authenticate, passing a block of SASL data for the security
		/// mechanism at the server side.
		/// </summary>
		public class SecureOk : IMethod, INonContentMethod, IClientMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 21;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Longstr _response;
			/// <summary>
			/// A block of opaque data passed to the security mechanism. The contents of this
			/// data are defined by the SASL security mechanism.
			/// </summary>
			public Longstr Response
			{
				get => _response;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_response = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_response = new Longstr(reader.ReadLongString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongString(_response.Value);
			}
		}

		/// <summary>
		/// This method proposes a set of connection configuration values to the client. The
		/// client can accept and/or adjust these.
		/// </summary>
		public class Tune : IMethod, INonContentMethod, IRespond<TuneOk>, IServerMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 30;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public TuneOk Respond(TuneOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(TuneOk) };
			}

			private Short _channelMax;
			/// <summary>
			/// Specifies highest channel number that the server permits.  Usable channel numbers
			/// are in the range 1..channel-max.  Zero indicates no specified limit.
			/// </summary>
			public Short ChannelMax
			{
				get => _channelMax;
				set
				{
					_channelMax = value;
				}
			}

			private Long _frameMax;
			/// <summary>
			/// The largest frame size that the server proposes for the connection, including
			/// frame header and end-byte.  The client can negotiate a lower value. Zero means
			/// that the server does not impose any specific limit but may reject very large
			/// frames if it cannot allocate resources for them.
			/// </summary>
			public Long FrameMax
			{
				get => _frameMax;
				set
				{
					_frameMax = value;
				}
			}

			private Short _heartbeat;
			/// <summary>
			/// The delay, in seconds, of the connection heartbeat that the server wants.
			/// Zero means the server does not want a heartbeat.
			/// </summary>
			public Short Heartbeat
			{
				get => _heartbeat;
				set
				{
					_heartbeat = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_channelMax = new Short(reader.ReadShortInteger());
				_frameMax = new Long(reader.ReadLongInteger());
				_heartbeat = new Short(reader.ReadShortInteger());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_channelMax.Value);
				writer.WriteLongInteger(_frameMax.Value);
				writer.WriteShortInteger(_heartbeat.Value);
			}
		}

		/// <summary>
		/// This method sends the client's connection tuning parameters to the server.
		/// Certain fields are negotiated, others provide capability information.
		/// </summary>
		public class TuneOk : IMethod, INonContentMethod, IClientMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 31;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Short _channelMax;
			/// <summary>
			/// The maximum total number of channels that the client will use per connection.
			/// </summary>
			public Short ChannelMax
			{
				get => _channelMax;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));

					_channelMax = value;
				}
			}

			private Long _frameMax;
			/// <summary>
			/// The largest frame size that the client and server will use for the connection.
			/// Zero means that the client does not impose any specific limit but may reject
			/// very large frames if it cannot allocate resources for them. Note that the
			/// frame-max limit applies principally to content frames, where large contents can
			/// be broken into frames of arbitrary size.
			/// </summary>
			public Long FrameMax
			{
				get => _frameMax;
				set
				{
					_frameMax = value;
				}
			}

			private Short _heartbeat;
			/// <summary>
			/// The delay, in seconds, of the connection heartbeat that the client wants. Zero
			/// means the client does not want a heartbeat.
			/// </summary>
			public Short Heartbeat
			{
				get => _heartbeat;
				set
				{
					_heartbeat = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_channelMax = new Short(reader.ReadShortInteger());
				_frameMax = new Long(reader.ReadLongInteger());
				_heartbeat = new Short(reader.ReadShortInteger());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_channelMax.Value);
				writer.WriteLongInteger(_frameMax.Value);
				writer.WriteShortInteger(_heartbeat.Value);
			}
		}

		/// <summary>
		/// This method opens a connection to a virtual host, which is a collection of
		/// resources, and acts to separate multiple application domains within a server.
		/// The server may apply arbitrary limits per virtual host, such as the number
		/// of each type of entity that may be used, per connection and/or in total.
		/// </summary>
		public class Open : IMethod, INonContentMethod, IRespond<OpenOk>, IClientMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 40;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public OpenOk Respond(OpenOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(OpenOk) };
			}

			private Path _virtualHost;
			/// <summary>
			/// The name of the virtual host to work with.
			/// </summary>
			public Path VirtualHost
			{
				get => _virtualHost;
				set
				{
					_virtualHost = value;
				}
			}

			private Shortstr _reserved1;
			public Shortstr Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private Bit _reserved2;
			public Bit Reserved2
			{
				get => _reserved2;
				set
				{
					_reserved2 = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_virtualHost = new Path(reader.ReadShortString());
				_reserved1 = new Shortstr(reader.ReadShortString());
				_reserved2 = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_virtualHost.Value);
				writer.WriteShortString(_reserved1.Value);
				writer.WriteBit(_reserved2.Value);
			}
		}

		/// <summary>
		/// This method signals to the client that the connection is ready for use.
		/// </summary>
		public class OpenOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 41;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Shortstr _reserved1;
			public Shortstr Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Shortstr(reader.ReadShortString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_reserved1.Value);
			}
		}

		/// <summary>
		/// This method indicates that the sender wants to close the connection. This may be
		/// due to internal conditions (e.g. a forced shut-down) or due to an error handling
		/// a specific method, i.e. an exception. When a close is due to an exception, the
		/// sender provides the class and method id of the method which caused the exception.
		/// </summary>
		public class Close : IMethod, INonContentMethod, IRespond<CloseOk>, IServerMethod, IClientMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 50;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public CloseOk Respond(CloseOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(CloseOk) };
			}

			private ReplyCode _replyCode;
			public ReplyCode ReplyCode
			{
				get => _replyCode;
				set
				{
					_replyCode = value;
				}
			}

			private ReplyText _replyText;
			public ReplyText ReplyText
			{
				get => _replyText;
				set
				{
					_replyText = value;
				}
			}

			private ClassId _classId;
			/// <summary>
			/// When the close is provoked by a method exception, this is the class of the
			/// method.
			/// </summary>
			public ClassId ClassId
			{
				get => _classId;
				set
				{
					_classId = value;
				}
			}

			private MethodId _methodId;
			/// <summary>
			/// When the close is provoked by a method exception, this is the ID of the method.
			/// </summary>
			public MethodId MethodId
			{
				get => _methodId;
				set
				{
					_methodId = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_replyCode = new ReplyCode(reader.ReadShortInteger());
				_replyText = new ReplyText(reader.ReadShortString());
				_classId = new ClassId(reader.ReadShortInteger());
				_methodId = new MethodId(reader.ReadShortInteger());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_replyCode.Value);
				writer.WriteShortString(_replyText.Value);
				writer.WriteShortInteger(_classId.Value);
				writer.WriteShortInteger(_methodId.Value);
			}
		}

		/// <summary>
		/// This method confirms a Connection.Close method and tells the recipient that it is
		/// safe to release resources for the connection and close the socket.
		/// </summary>
		public class CloseOk : IMethod, INonContentMethod, IServerMethod, IClientMethod
		{
			public int ProtocolClassId => 10;
			public int ProtocolMethodId => 51;

			public bool SentOnValidChannel(int channel)
			{
				return channel == 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}
	}

	/// <summary>
	/// The channel class provides methods for a client to establish a channel to a
	/// server and for both peers to operate the channel thereafter.
	/// </summary>
	/// <example>
	/// 
	///       channel             = open-channel *use-channel close-channel
	///       open-channel        = C:OPEN S:OPEN-OK
	///       use-channel         = C:FLOW S:FLOW-OK
	///                           / S:FLOW C:FLOW-OK
	///                           / functional-class
	///       close-channel       = C:CLOSE S:CLOSE-OK
	///                           / S:CLOSE C:CLOSE-OK
	/// </example>
	public class Channel
	{
		/// <summary>
		/// This method opens a channel to the server.
		/// </summary>
		public class Open : IMethod, INonContentMethod, IRespond<OpenOk>, IClientMethod
		{
			public int ProtocolClassId => 20;
			public int ProtocolMethodId => 10;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public OpenOk Respond(OpenOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(OpenOk) };
			}

			private Shortstr _reserved1;
			public Shortstr Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Shortstr(reader.ReadShortString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_reserved1.Value);
			}
		}

		/// <summary>
		/// This method signals to the client that the channel is ready for use.
		/// </summary>
		public class OpenOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 20;
			public int ProtocolMethodId => 11;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Longstr _reserved1;
			public Longstr Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Longstr(reader.ReadLongString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongString(_reserved1.Value);
			}
		}

		/// <summary>
		/// This method asks the peer to pause or restart the flow of content data sent by
		/// a consumer. This is a simple flow-control mechanism that a peer can use to avoid
		/// overflowing its queues or otherwise finding itself receiving more messages than
		/// it can process. Note that this method is not intended for window control. It does
		/// not affect contents returned by Basic.Get-Ok methods.
		/// </summary>
		public class Flow : IMethod, INonContentMethod, IRespond<FlowOk>, IServerMethod, IClientMethod
		{
			public int ProtocolClassId => 20;
			public int ProtocolMethodId => 20;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public FlowOk Respond(FlowOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(FlowOk) };
			}

			private Bit _active;
			/// <summary>
			/// If 1, the peer starts sending content frames. If 0, the peer stops sending
			/// content frames.
			/// </summary>
			public Bit Active
			{
				get => _active;
				set
				{
					_active = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_active = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteBit(_active.Value);
			}
		}

		/// <summary>
		/// Confirms to the peer that a flow command was received and processed.
		/// </summary>
		public class FlowOk : IMethod, INonContentMethod, IServerMethod, IClientMethod
		{
			public int ProtocolClassId => 20;
			public int ProtocolMethodId => 21;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Bit _active;
			/// <summary>
			/// Confirms the setting of the processed flow method: 1 means the peer will start
			/// sending or continue to send content frames; 0 means it will not.
			/// </summary>
			public Bit Active
			{
				get => _active;
				set
				{
					_active = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_active = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteBit(_active.Value);
			}
		}

		/// <summary>
		/// This method indicates that the sender wants to close the channel. This may be due to
		/// internal conditions (e.g. a forced shut-down) or due to an error handling a specific
		/// method, i.e. an exception. When a close is due to an exception, the sender provides
		/// the class and method id of the method which caused the exception.
		/// </summary>
		public class Close : IMethod, INonContentMethod, IRespond<CloseOk>, IServerMethod, IClientMethod
		{
			public int ProtocolClassId => 20;
			public int ProtocolMethodId => 40;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public CloseOk Respond(CloseOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(CloseOk) };
			}

			private ReplyCode _replyCode;
			public ReplyCode ReplyCode
			{
				get => _replyCode;
				set
				{
					_replyCode = value;
				}
			}

			private ReplyText _replyText;
			public ReplyText ReplyText
			{
				get => _replyText;
				set
				{
					_replyText = value;
				}
			}

			private ClassId _classId;
			/// <summary>
			/// When the close is provoked by a method exception, this is the class of the
			/// method.
			/// </summary>
			public ClassId ClassId
			{
				get => _classId;
				set
				{
					_classId = value;
				}
			}

			private MethodId _methodId;
			/// <summary>
			/// When the close is provoked by a method exception, this is the ID of the method.
			/// </summary>
			public MethodId MethodId
			{
				get => _methodId;
				set
				{
					_methodId = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_replyCode = new ReplyCode(reader.ReadShortInteger());
				_replyText = new ReplyText(reader.ReadShortString());
				_classId = new ClassId(reader.ReadShortInteger());
				_methodId = new MethodId(reader.ReadShortInteger());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_replyCode.Value);
				writer.WriteShortString(_replyText.Value);
				writer.WriteShortInteger(_classId.Value);
				writer.WriteShortInteger(_methodId.Value);
			}
		}

		/// <summary>
		/// This method confirms a Channel.Close method and tells the recipient that it is safe
		/// to release resources for the channel.
		/// </summary>
		public class CloseOk : IMethod, INonContentMethod, IServerMethod, IClientMethod
		{
			public int ProtocolClassId => 20;
			public int ProtocolMethodId => 41;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}
	}

	/// <summary>
	/// Exchanges match and distribute messages across queues. Exchanges can be configured in
	/// the server or declared at runtime.
	/// </summary>
	/// <example>
	/// 
	///       exchange            = C:DECLARE  S:DECLARE-OK
	///                           / C:DELETE   S:DELETE-OK
	/// </example>
	public class Exchange
	{
		/// <summary>
		/// This method creates an exchange if it does not already exist, and if the exchange
		/// exists, verifies that it is of the correct and expected class.
		/// </summary>
		public class Declare : IMethod, INonContentMethod, IRespond<DeclareOk>, IClientMethod
		{
			public int ProtocolClassId => 40;
			public int ProtocolMethodId => 10;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public DeclareOk Respond(DeclareOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(DeclareOk) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private ExchangeName _exchange;
			public ExchangeName Exchange
			{
				get => _exchange;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_exchange = value;
				}
			}

			private Shortstr _type;
			/// <summary>
			/// Each exchange belongs to one of a set of exchange types implemented by the
			/// server. The exchange types define the functionality of the exchange - i.e. how
			/// messages are routed through it. It is not valid or meaningful to attempt to
			/// change the type of an existing exchange.
			/// </summary>
			public Shortstr Type
			{
				get => _type;
				set
				{
					_type = value;
				}
			}

			private Bit _passive;
			/// <summary>
			/// If set, the server will reply with Declare-Ok if the exchange already
			/// exists with the same name, and raise an error if not.  The client can
			/// use this to check whether an exchange exists without modifying the
			/// server state. When set, all other method fields except name and no-wait
			/// are ignored.  A declare with both passive and no-wait has no effect.
			/// Arguments are compared for semantic equivalence.
			/// </summary>
			public Bit Passive
			{
				get => _passive;
				set
				{
					_passive = value;
				}
			}

			private Bit _durable;
			/// <summary>
			/// If set when creating a new exchange, the exchange will be marked as durable.
			/// Durable exchanges remain active when a server restarts. Non-durable exchanges
			/// (transient exchanges) are purged if/when a server restarts.
			/// </summary>
			public Bit Durable
			{
				get => _durable;
				set
				{
					_durable = value;
				}
			}

			private Bit _reserved2;
			public Bit Reserved2
			{
				get => _reserved2;
				set
				{
					_reserved2 = value;
				}
			}

			private Bit _reserved3;
			public Bit Reserved3
			{
				get => _reserved3;
				set
				{
					_reserved3 = value;
				}
			}

			private NoWait _noWait;
			public NoWait NoWait
			{
				get => _noWait;
				set
				{
					_noWait = value;
				}
			}

			private Table _arguments;
			/// <summary>
			/// A set of arguments for the declaration. The syntax and semantics of these
			/// arguments depends on the server implementation.
			/// </summary>
			public Table Arguments
			{
				get => _arguments;
				set
				{
					_arguments = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_exchange = new ExchangeName(reader.ReadShortString());
				_type = new Shortstr(reader.ReadShortString());
				_passive = new Bit(reader.ReadBit());
				_durable = new Bit(reader.ReadBit());
				_reserved2 = new Bit(reader.ReadBit());
				_reserved3 = new Bit(reader.ReadBit());
				_noWait = new NoWait(reader.ReadBit());
				_arguments = new Table(reader.ReadTable());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_exchange.Value);
				writer.WriteShortString(_type.Value);
				writer.WriteBit(_passive.Value);
				writer.WriteBit(_durable.Value);
				writer.WriteBit(_reserved2.Value);
				writer.WriteBit(_reserved3.Value);
				writer.WriteBit(_noWait.Value);
				writer.WriteTable(_arguments.Value);
			}
		}

		/// <summary>
		/// This method confirms a Declare method and confirms the name of the exchange,
		/// essential for automatically-named exchanges.
		/// </summary>
		public class DeclareOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 40;
			public int ProtocolMethodId => 11;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method deletes an exchange. When an exchange is deleted all queue bindings on
		/// the exchange are cancelled.
		/// </summary>
		public class Delete : IMethod, INonContentMethod, IRespond<DeleteOk>, IClientMethod
		{
			public int ProtocolClassId => 40;
			public int ProtocolMethodId => 20;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public DeleteOk Respond(DeleteOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(DeleteOk) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private ExchangeName _exchange;
			public ExchangeName Exchange
			{
				get => _exchange;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_exchange = value;
				}
			}

			private Bit _ifUnused;
			/// <summary>
			/// If set, the server will only delete the exchange if it has no queue bindings. If
			/// the exchange has queue bindings the server does not delete it but raises a
			/// channel exception instead.
			/// </summary>
			public Bit IfUnused
			{
				get => _ifUnused;
				set
				{
					_ifUnused = value;
				}
			}

			private NoWait _noWait;
			public NoWait NoWait
			{
				get => _noWait;
				set
				{
					_noWait = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_exchange = new ExchangeName(reader.ReadShortString());
				_ifUnused = new Bit(reader.ReadBit());
				_noWait = new NoWait(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_exchange.Value);
				writer.WriteBit(_ifUnused.Value);
				writer.WriteBit(_noWait.Value);
			}
		}

		/// <summary>
		/// This method confirms the deletion of an exchange.
		/// </summary>
		public class DeleteOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 40;
			public int ProtocolMethodId => 21;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}
	}

	/// <summary>
	/// Queues store and forward messages. Queues can be configured in the server or created at
	/// runtime. Queues must be attached to at least one exchange in order to receive messages
	/// from publishers.
	/// </summary>
	/// <example>
	/// 
	///       queue               = C:DECLARE  S:DECLARE-OK
	///                           / C:BIND     S:BIND-OK
	///                           / C:UNBIND   S:UNBIND-OK
	///                           / C:PURGE    S:PURGE-OK
	///                           / C:DELETE   S:DELETE-OK
	/// </example>
	public class Queue
	{
		/// <summary>
		/// This method creates or checks a queue. When creating a new queue the client can
		/// specify various properties that control the durability of the queue and its
		/// contents, and the level of sharing for the queue.
		/// </summary>
		public class Declare : IMethod, INonContentMethod, IRespond<DeclareOk>, IClientMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 10;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public DeclareOk Respond(DeclareOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(DeclareOk) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private QueueName _queue;
			public QueueName Queue
			{
				get => _queue;
				set
				{
					_queue = value;
				}
			}

			private Bit _passive;
			/// <summary>
			/// If set, the server will reply with Declare-Ok if the queue already
			/// exists with the same name, and raise an error if not.  The client can
			/// use this to check whether a queue exists without modifying the
			/// server state.  When set, all other method fields except name and no-wait
			/// are ignored.  A declare with both passive and no-wait has no effect.
			/// Arguments are compared for semantic equivalence.
			/// </summary>
			public Bit Passive
			{
				get => _passive;
				set
				{
					_passive = value;
				}
			}

			private Bit _durable;
			/// <summary>
			/// If set when creating a new queue, the queue will be marked as durable. Durable
			/// queues remain active when a server restarts. Non-durable queues (transient
			/// queues) are purged if/when a server restarts. Note that durable queues do not
			/// necessarily hold persistent messages, although it does not make sense to send
			/// persistent messages to a transient queue.
			/// </summary>
			public Bit Durable
			{
				get => _durable;
				set
				{
					_durable = value;
				}
			}

			private Bit _exclusive;
			/// <summary>
			/// Exclusive queues may only be accessed by the current connection, and are
			/// deleted when that connection closes.  Passive declaration of an exclusive
			/// queue by other connections are not allowed.
			/// </summary>
			public Bit Exclusive
			{
				get => _exclusive;
				set
				{
					_exclusive = value;
				}
			}

			private Bit _autoDelete;
			/// <summary>
			/// If set, the queue is deleted when all consumers have finished using it.  The last
			/// consumer can be cancelled either explicitly or because its channel is closed. If
			/// there was no consumer ever on the queue, it won't be deleted.  Applications can
			/// explicitly delete auto-delete queues using the Delete method as normal.
			/// </summary>
			public Bit AutoDelete
			{
				get => _autoDelete;
				set
				{
					_autoDelete = value;
				}
			}

			private NoWait _noWait;
			public NoWait NoWait
			{
				get => _noWait;
				set
				{
					_noWait = value;
				}
			}

			private Table _arguments;
			/// <summary>
			/// A set of arguments for the declaration. The syntax and semantics of these
			/// arguments depends on the server implementation.
			/// </summary>
			public Table Arguments
			{
				get => _arguments;
				set
				{
					_arguments = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_queue = new QueueName(reader.ReadShortString());
				_passive = new Bit(reader.ReadBit());
				_durable = new Bit(reader.ReadBit());
				_exclusive = new Bit(reader.ReadBit());
				_autoDelete = new Bit(reader.ReadBit());
				_noWait = new NoWait(reader.ReadBit());
				_arguments = new Table(reader.ReadTable());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_queue.Value);
				writer.WriteBit(_passive.Value);
				writer.WriteBit(_durable.Value);
				writer.WriteBit(_exclusive.Value);
				writer.WriteBit(_autoDelete.Value);
				writer.WriteBit(_noWait.Value);
				writer.WriteTable(_arguments.Value);
			}
		}

		/// <summary>
		/// This method confirms a Declare method and confirms the name of the queue, essential
		/// for automatically-named queues.
		/// </summary>
		public class DeclareOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 11;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private QueueName _queue;
			/// <summary>
			/// Reports the name of the queue. If the server generated a queue name, this field
			/// contains that name.
			/// </summary>
			public QueueName Queue
			{
				get => _queue;
				set
				{
					Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
					_queue = value;
				}
			}

			private MessageCount _messageCount;
			public MessageCount MessageCount
			{
				get => _messageCount;
				set
				{
					_messageCount = value;
				}
			}

			private Long _consumerCount;
			/// <summary>
			/// Reports the number of active consumers for the queue. Note that consumers can
			/// suspend activity (Channel.Flow) in which case they do not appear in this count.
			/// </summary>
			public Long ConsumerCount
			{
				get => _consumerCount;
				set
				{
					_consumerCount = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_queue = new QueueName(reader.ReadShortString());
				_messageCount = new MessageCount(reader.ReadLongInteger());
				_consumerCount = new Long(reader.ReadLongInteger());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_queue.Value);
				writer.WriteLongInteger(_messageCount.Value);
				writer.WriteLongInteger(_consumerCount.Value);
			}
		}

		/// <summary>
		/// This method binds a queue to an exchange. Until a queue is bound it will not
		/// receive any messages. In a classic messaging model, store-and-forward queues
		/// are bound to a direct exchange and subscription queues are bound to a topic
		/// exchange.
		/// </summary>
		public class Bind : IMethod, INonContentMethod, IRespond<BindOk>, IClientMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 20;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public BindOk Respond(BindOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(BindOk) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private QueueName _queue;
			/// <summary>
			/// Specifies the name of the queue to bind.
			/// </summary>
			public QueueName Queue
			{
				get => _queue;
				set
				{
					_queue = value;
				}
			}

			private ExchangeName _exchange;
			public ExchangeName Exchange
			{
				get => _exchange;
				set
				{
					_exchange = value;
				}
			}

			private Shortstr _routingKey;
			/// <summary>
			/// Specifies the routing key for the binding. The routing key is used for routing
			/// messages depending on the exchange configuration. Not all exchanges use a
			/// routing key - refer to the specific exchange documentation.  If the queue name
			/// is empty, the server uses the last queue declared on the channel.  If the
			/// routing key is also empty, the server uses this queue name for the routing
			/// key as well.  If the queue name is provided but the routing key is empty, the
			/// server does the binding with that empty routing key.  The meaning of empty
			/// routing keys depends on the exchange implementation.
			/// </summary>
			public Shortstr RoutingKey
			{
				get => _routingKey;
				set
				{
					_routingKey = value;
				}
			}

			private NoWait _noWait;
			public NoWait NoWait
			{
				get => _noWait;
				set
				{
					_noWait = value;
				}
			}

			private Table _arguments;
			/// <summary>
			/// A set of arguments for the binding. The syntax and semantics of these arguments
			/// depends on the exchange class.
			/// </summary>
			public Table Arguments
			{
				get => _arguments;
				set
				{
					_arguments = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_queue = new QueueName(reader.ReadShortString());
				_exchange = new ExchangeName(reader.ReadShortString());
				_routingKey = new Shortstr(reader.ReadShortString());
				_noWait = new NoWait(reader.ReadBit());
				_arguments = new Table(reader.ReadTable());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_queue.Value);
				writer.WriteShortString(_exchange.Value);
				writer.WriteShortString(_routingKey.Value);
				writer.WriteBit(_noWait.Value);
				writer.WriteTable(_arguments.Value);
			}
		}

		/// <summary>
		/// This method confirms that the bind was successful.
		/// </summary>
		public class BindOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 21;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method unbinds a queue from an exchange.
		/// </summary>
		public class Unbind : IMethod, INonContentMethod, IRespond<UnbindOk>, IClientMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 50;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public UnbindOk Respond(UnbindOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(UnbindOk) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private QueueName _queue;
			/// <summary>
			/// Specifies the name of the queue to unbind.
			/// </summary>
			public QueueName Queue
			{
				get => _queue;
				set
				{
					_queue = value;
				}
			}

			private ExchangeName _exchange;
			/// <summary>
			/// The name of the exchange to unbind from.
			/// </summary>
			public ExchangeName Exchange
			{
				get => _exchange;
				set
				{
					_exchange = value;
				}
			}

			private Shortstr _routingKey;
			/// <summary>
			/// Specifies the routing key of the binding to unbind.
			/// </summary>
			public Shortstr RoutingKey
			{
				get => _routingKey;
				set
				{
					_routingKey = value;
				}
			}

			private Table _arguments;
			/// <summary>
			/// Specifies the arguments of the binding to unbind.
			/// </summary>
			public Table Arguments
			{
				get => _arguments;
				set
				{
					_arguments = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_queue = new QueueName(reader.ReadShortString());
				_exchange = new ExchangeName(reader.ReadShortString());
				_routingKey = new Shortstr(reader.ReadShortString());
				_arguments = new Table(reader.ReadTable());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_queue.Value);
				writer.WriteShortString(_exchange.Value);
				writer.WriteShortString(_routingKey.Value);
				writer.WriteTable(_arguments.Value);
			}
		}

		/// <summary>
		/// This method confirms that the unbind was successful.
		/// </summary>
		public class UnbindOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 51;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method removes all messages from a queue which are not awaiting
		/// acknowledgment.
		/// </summary>
		public class Purge : IMethod, INonContentMethod, IRespond<PurgeOk>, IClientMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 30;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public PurgeOk Respond(PurgeOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(PurgeOk) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private QueueName _queue;
			/// <summary>
			/// Specifies the name of the queue to purge.
			/// </summary>
			public QueueName Queue
			{
				get => _queue;
				set
				{
					_queue = value;
				}
			}

			private NoWait _noWait;
			public NoWait NoWait
			{
				get => _noWait;
				set
				{
					_noWait = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_queue = new QueueName(reader.ReadShortString());
				_noWait = new NoWait(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_queue.Value);
				writer.WriteBit(_noWait.Value);
			}
		}

		/// <summary>
		/// This method confirms the purge of a queue.
		/// </summary>
		public class PurgeOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 31;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private MessageCount _messageCount;
			/// <summary>
			/// Reports the number of messages purged.
			/// </summary>
			public MessageCount MessageCount
			{
				get => _messageCount;
				set
				{
					_messageCount = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_messageCount = new MessageCount(reader.ReadLongInteger());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongInteger(_messageCount.Value);
			}
		}

		/// <summary>
		/// This method deletes a queue. When a queue is deleted any pending messages are sent
		/// to a dead-letter queue if this is defined in the server configuration, and all
		/// consumers on the queue are cancelled.
		/// </summary>
		public class Delete : IMethod, INonContentMethod, IRespond<DeleteOk>, IClientMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 40;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public DeleteOk Respond(DeleteOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(DeleteOk) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private QueueName _queue;
			/// <summary>
			/// Specifies the name of the queue to delete.
			/// </summary>
			public QueueName Queue
			{
				get => _queue;
				set
				{
					_queue = value;
				}
			}

			private Bit _ifUnused;
			/// <summary>
			/// If set, the server will only delete the queue if it has no consumers. If the
			/// queue has consumers the server does does not delete it but raises a channel
			/// exception instead.
			/// </summary>
			public Bit IfUnused
			{
				get => _ifUnused;
				set
				{
					_ifUnused = value;
				}
			}

			private Bit _ifEmpty;
			/// <summary>
			/// If set, the server will only delete the queue if it has no messages.
			/// </summary>
			public Bit IfEmpty
			{
				get => _ifEmpty;
				set
				{
					_ifEmpty = value;
				}
			}

			private NoWait _noWait;
			public NoWait NoWait
			{
				get => _noWait;
				set
				{
					_noWait = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_queue = new QueueName(reader.ReadShortString());
				_ifUnused = new Bit(reader.ReadBit());
				_ifEmpty = new Bit(reader.ReadBit());
				_noWait = new NoWait(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_queue.Value);
				writer.WriteBit(_ifUnused.Value);
				writer.WriteBit(_ifEmpty.Value);
				writer.WriteBit(_noWait.Value);
			}
		}

		/// <summary>
		/// This method confirms the deletion of a queue.
		/// </summary>
		public class DeleteOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 50;
			public int ProtocolMethodId => 41;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private MessageCount _messageCount;
			/// <summary>
			/// Reports the number of messages deleted.
			/// </summary>
			public MessageCount MessageCount
			{
				get => _messageCount;
				set
				{
					_messageCount = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_messageCount = new MessageCount(reader.ReadLongInteger());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongInteger(_messageCount.Value);
			}
		}
	}

	/// <summary>
	/// The Basic class provides methods that support an industry-standard messaging model.
	/// </summary>
	/// <example>
	/// 
	///       basic               = C:QOS S:QOS-OK
	///                           / C:CONSUME S:CONSUME-OK
	///                           / C:CANCEL S:CANCEL-OK
	///                           / C:PUBLISH content
	///                           / S:RETURN content
	///                           / S:DELIVER content
	///                           / C:GET ( S:GET-OK content / S:GET-EMPTY )
	///                           / C:ACK
	///                           / C:REJECT
	///                           / C:RECOVER-ASYNC
	///                           / C:RECOVER S:RECOVER-OK
	/// </example>
	public class Basic
	{
		public class ContentHeader : IContentHeader
		{
			public int ClassId { get; } = 60;

			public long BodySize { get; private set; }

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public bool HasContentType { get; private set; }
			private Shortstr _contentType;
			public Shortstr ContentType
			{
				get => _contentType;
				set
				{
					_contentType = value;
				}
			}

			public bool HasContentEncoding { get; private set; }
			private Shortstr _contentEncoding;
			public Shortstr ContentEncoding
			{
				get => _contentEncoding;
				set
				{
					_contentEncoding = value;
				}
			}

			public bool HasHeaders { get; private set; }
			private Table _headers;
			public Table Headers
			{
				get => _headers;
				set
				{
					_headers = value;
				}
			}

			public bool HasDeliveryMode { get; private set; }
			private Octet _deliveryMode;
			public Octet DeliveryMode
			{
				get => _deliveryMode;
				set
				{
					_deliveryMode = value;
				}
			}

			public bool HasPriority { get; private set; }
			private Octet _priority;
			public Octet Priority
			{
				get => _priority;
				set
				{
					_priority = value;
				}
			}

			public bool HasCorrelationId { get; private set; }
			private Shortstr _correlationId;
			public Shortstr CorrelationId
			{
				get => _correlationId;
				set
				{
					_correlationId = value;
				}
			}

			public bool HasReplyTo { get; private set; }
			private Shortstr _replyTo;
			public Shortstr ReplyTo
			{
				get => _replyTo;
				set
				{
					_replyTo = value;
				}
			}

			public bool HasExpiration { get; private set; }
			private Shortstr _expiration;
			public Shortstr Expiration
			{
				get => _expiration;
				set
				{
					_expiration = value;
				}
			}

			public bool HasMessageId { get; private set; }
			private Shortstr _messageId;
			public Shortstr MessageId
			{
				get => _messageId;
				set
				{
					_messageId = value;
				}
			}

			public bool HasTimestamp { get; private set; }
			private Timestamp _timestamp;
			public Timestamp Timestamp
			{
				get => _timestamp;
				set
				{
					_timestamp = value;
				}
			}

			public bool HasType { get; private set; }
			private Shortstr _type;
			public Shortstr Type
			{
				get => _type;
				set
				{
					_type = value;
				}
			}

			public bool HasUserId { get; private set; }
			private Shortstr _userId;
			public Shortstr UserId
			{
				get => _userId;
				set
				{
					_userId = value;
				}
			}

			public bool HasAppId { get; private set; }
			private Shortstr _appId;
			public Shortstr AppId
			{
				get => _appId;
				set
				{
					_appId = value;
				}
			}

			public bool HasReserved { get; private set; }
			private Shortstr _reserved;
			public Shortstr Reserved
			{
				get => _reserved;
				set
				{
					_reserved = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				BodySize = reader.ReadLongLongInteger();
				var propertyFlags = reader.ReadPropertyFlags(); 
				if (propertyFlags.Length < 14)
				{
					throw new ProtocolViolationException("There are more fields than property flags.");
				}

				HasContentType = propertyFlags[0];
				if (HasContentType)
				{
					_contentType = new Shortstr(reader.ReadShortString());
				}

				HasContentEncoding = propertyFlags[1];
				if (HasContentEncoding)
				{
					_contentEncoding = new Shortstr(reader.ReadShortString());
				}

				HasHeaders = propertyFlags[2];
				if (HasHeaders)
				{
					_headers = new Table(reader.ReadTable());
				}

				HasDeliveryMode = propertyFlags[3];
				if (HasDeliveryMode)
				{
					_deliveryMode = new Octet(reader.ReadByte());
				}

				HasPriority = propertyFlags[4];
				if (HasPriority)
				{
					_priority = new Octet(reader.ReadByte());
				}

				HasCorrelationId = propertyFlags[5];
				if (HasCorrelationId)
				{
					_correlationId = new Shortstr(reader.ReadShortString());
				}

				HasReplyTo = propertyFlags[6];
				if (HasReplyTo)
				{
					_replyTo = new Shortstr(reader.ReadShortString());
				}

				HasExpiration = propertyFlags[7];
				if (HasExpiration)
				{
					_expiration = new Shortstr(reader.ReadShortString());
				}

				HasMessageId = propertyFlags[8];
				if (HasMessageId)
				{
					_messageId = new Shortstr(reader.ReadShortString());
				}

				HasTimestamp = propertyFlags[9];
				if (HasTimestamp)
				{
					_timestamp = new Timestamp(reader.ReadTimestamp());
				}

				HasType = propertyFlags[10];
				if (HasType)
				{
					_type = new Shortstr(reader.ReadShortString());
				}

				HasUserId = propertyFlags[11];
				if (HasUserId)
				{
					_userId = new Shortstr(reader.ReadShortString());
				}

				HasAppId = propertyFlags[12];
				if (HasAppId)
				{
					_appId = new Shortstr(reader.ReadShortString());
				}

				HasReserved = propertyFlags[13];
				if (HasReserved)
				{
					_reserved = new Shortstr(reader.ReadShortString());
				}
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ClassId);
				writer.WriteShortUnsignedInteger(0);
				writer.WriteLongLongInteger(BodySize);

				var propertyFlags = new [] 
				{
					HasContentType,
					HasContentEncoding,
					HasHeaders,
					HasDeliveryMode,
					HasPriority,
					HasCorrelationId,
					HasReplyTo,
					HasExpiration,
					HasMessageId,
					HasTimestamp,
					HasType,
					HasUserId,
					HasAppId,
					HasReserved
				};

				writer.WritePropertyFlags(propertyFlags);

				if (HasContentType)
				{
					writer.WriteShortString(_contentType.Value);
				}

				if (HasContentEncoding)
				{
					writer.WriteShortString(_contentEncoding.Value);
				}

				if (HasHeaders)
				{
					writer.WriteTable(_headers.Value);
				}

				if (HasDeliveryMode)
				{
					writer.WriteByte(_deliveryMode.Value);
				}

				if (HasPriority)
				{
					writer.WriteByte(_priority.Value);
				}

				if (HasCorrelationId)
				{
					writer.WriteShortString(_correlationId.Value);
				}

				if (HasReplyTo)
				{
					writer.WriteShortString(_replyTo.Value);
				}

				if (HasExpiration)
				{
					writer.WriteShortString(_expiration.Value);
				}

				if (HasMessageId)
				{
					writer.WriteShortString(_messageId.Value);
				}

				if (HasTimestamp)
				{
					writer.WriteTimestamp(_timestamp.Value);
				}

				if (HasType)
				{
					writer.WriteShortString(_type.Value);
				}

				if (HasUserId)
				{
					writer.WriteShortString(_userId.Value);
				}

				if (HasAppId)
				{
					writer.WriteShortString(_appId.Value);
				}

				if (HasReserved)
				{
					writer.WriteShortString(_reserved.Value);
				}
			}
		}

		/// <summary>
		/// This method requests a specific quality of service. The QoS can be specified for the
		/// current channel or for all channels on the connection. The particular properties and
		/// semantics of a qos method always depend on the content class semantics. Though the
		/// qos method could in principle apply to both peers, it is currently meaningful only
		/// for the server.
		/// </summary>
		public class Qos : IMethod, INonContentMethod, IRespond<QosOk>, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 10;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public QosOk Respond(QosOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(QosOk) };
			}

			private Long _prefetchSize;
			/// <summary>
			/// The client can request that messages be sent in advance so that when the client
			/// finishes processing a message, the following message is already held locally,
			/// rather than needing to be sent down the channel. Prefetching gives a performance
			/// improvement. This field specifies the prefetch window size in octets. The server
			/// will send a message in advance if it is equal to or smaller in size than the
			/// available prefetch size (and also falls into other prefetch limits). May be set
			/// to zero, meaning "no specific limit", although other prefetch limits may still
			/// apply. The prefetch-size is ignored if the no-ack option is set.
			/// </summary>
			public Long PrefetchSize
			{
				get => _prefetchSize;
				set
				{
					_prefetchSize = value;
				}
			}

			private Short _prefetchCount;
			/// <summary>
			/// Specifies a prefetch window in terms of whole messages. This field may be used
			/// in combination with the prefetch-size field; a message will only be sent in
			/// advance if both prefetch windows (and those at the channel and connection level)
			/// allow it. The prefetch-count is ignored if the no-ack option is set.
			/// </summary>
			public Short PrefetchCount
			{
				get => _prefetchCount;
				set
				{
					_prefetchCount = value;
				}
			}

			private Bit _global;
			/// <summary>
			/// By default the QoS settings apply to the current channel only. If this field is
			/// set, they are applied to the entire connection.
			/// </summary>
			public Bit Global
			{
				get => _global;
				set
				{
					_global = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_prefetchSize = new Long(reader.ReadLongInteger());
				_prefetchCount = new Short(reader.ReadShortInteger());
				_global = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongInteger(_prefetchSize.Value);
				writer.WriteShortInteger(_prefetchCount.Value);
				writer.WriteBit(_global.Value);
			}
		}

		/// <summary>
		/// This method tells the client that the requested QoS levels could be handled by the
		/// server. The requested QoS applies to all active consumers until a new QoS is
		/// defined.
		/// </summary>
		public class QosOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 11;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method asks the server to start a "consumer", which is a transient request for
		/// messages from a specific queue. Consumers last as long as the channel they were
		/// declared on, or until the client cancels them.
		/// </summary>
		public class Consume : IMethod, INonContentMethod, IRespond<ConsumeOk>, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 20;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public ConsumeOk Respond(ConsumeOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(ConsumeOk) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private QueueName _queue;
			/// <summary>
			/// Specifies the name of the queue to consume from.
			/// </summary>
			public QueueName Queue
			{
				get => _queue;
				set
				{
					_queue = value;
				}
			}

			private ConsumerTag _consumerTag;
			/// <summary>
			/// Specifies the identifier for the consumer. The consumer tag is local to a
			/// channel, so two clients can use the same consumer tags. If this field is
			/// empty the server will generate a unique tag.
			/// </summary>
			public ConsumerTag ConsumerTag
			{
				get => _consumerTag;
				set
				{
					_consumerTag = value;
				}
			}

			private NoLocal _noLocal;
			public NoLocal NoLocal
			{
				get => _noLocal;
				set
				{
					_noLocal = value;
				}
			}

			private NoAck _noAck;
			public NoAck NoAck
			{
				get => _noAck;
				set
				{
					_noAck = value;
				}
			}

			private Bit _exclusive;
			/// <summary>
			/// Request exclusive consumer access, meaning only this consumer can access the
			/// queue.
			/// </summary>
			public Bit Exclusive
			{
				get => _exclusive;
				set
				{
					_exclusive = value;
				}
			}

			private NoWait _noWait;
			public NoWait NoWait
			{
				get => _noWait;
				set
				{
					_noWait = value;
				}
			}

			private Table _arguments;
			/// <summary>
			/// A set of arguments for the consume. The syntax and semantics of these
			/// arguments depends on the server implementation.
			/// </summary>
			public Table Arguments
			{
				get => _arguments;
				set
				{
					_arguments = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_queue = new QueueName(reader.ReadShortString());
				_consumerTag = new ConsumerTag(reader.ReadShortString());
				_noLocal = new NoLocal(reader.ReadBit());
				_noAck = new NoAck(reader.ReadBit());
				_exclusive = new Bit(reader.ReadBit());
				_noWait = new NoWait(reader.ReadBit());
				_arguments = new Table(reader.ReadTable());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_queue.Value);
				writer.WriteShortString(_consumerTag.Value);
				writer.WriteBit(_noLocal.Value);
				writer.WriteBit(_noAck.Value);
				writer.WriteBit(_exclusive.Value);
				writer.WriteBit(_noWait.Value);
				writer.WriteTable(_arguments.Value);
			}
		}

		/// <summary>
		/// The server provides the client with a consumer tag, which is used by the client
		/// for methods called on the consumer at a later stage.
		/// </summary>
		public class ConsumeOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 21;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private ConsumerTag _consumerTag;
			/// <summary>
			/// Holds the consumer tag specified by the client or provided by the server.
			/// </summary>
			public ConsumerTag ConsumerTag
			{
				get => _consumerTag;
				set
				{
					_consumerTag = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_consumerTag = new ConsumerTag(reader.ReadShortString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_consumerTag.Value);
			}
		}

		/// <summary>
		/// This method cancels a consumer. This does not affect already delivered
		/// messages, but it does mean the server will not send any more messages for
		/// that consumer. The client may receive an arbitrary number of messages in
		/// between sending the cancel method and receiving the cancel-ok reply.
		/// </summary>
		public class Cancel : IMethod, INonContentMethod, IRespond<CancelOk>, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 30;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public CancelOk Respond(CancelOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(CancelOk) };
			}

			private ConsumerTag _consumerTag;
			public ConsumerTag ConsumerTag
			{
				get => _consumerTag;
				set
				{
					_consumerTag = value;
				}
			}

			private NoWait _noWait;
			public NoWait NoWait
			{
				get => _noWait;
				set
				{
					_noWait = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_consumerTag = new ConsumerTag(reader.ReadShortString());
				_noWait = new NoWait(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_consumerTag.Value);
				writer.WriteBit(_noWait.Value);
			}
		}

		/// <summary>
		/// This method confirms that the cancellation was completed.
		/// </summary>
		public class CancelOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 31;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private ConsumerTag _consumerTag;
			public ConsumerTag ConsumerTag
			{
				get => _consumerTag;
				set
				{
					_consumerTag = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_consumerTag = new ConsumerTag(reader.ReadShortString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_consumerTag.Value);
			}
		}

		/// <summary>
		/// This method publishes a message to a specific exchange. The message will be routed
		/// to queues as defined by the exchange configuration and distributed to any active
		/// consumers when the transaction, if any, is committed.
		/// </summary>
		public class Publish : IMethod, IContentMethod<Basic.ContentHeader>, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 40;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Basic.ContentHeader ContentHeader { get; private set; }

			public void SetContentHeader(IContentHeader contentHeader)
			{
				if (typeof(Basic.ContentHeader) != contentHeader.GetType())
				{
					throw new UnexpectedFrameException($"Did not expect {contentHeader.GetType()}, expected {typeof(Basic.ContentHeader)}.");
				}
				ContentHeader = (Basic.ContentHeader) contentHeader;
			}

			public byte[] ContentBody => _contentBodyFragments.SelectMany(fragment => fragment.Payload).ToArray();

			public IContentBody[] ContentBodyFragments => _contentBodyFragments.ToArray();

			private readonly List<IContentBody> _contentBodyFragments = new List<IContentBody>();
			public void AddContentBody(IContentBody contentBody)
			{
				_contentBodyFragments.Add(contentBody);
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private ExchangeName _exchange;
			/// <summary>
			/// Specifies the name of the exchange to publish to. The exchange name can be
			/// empty, meaning the default exchange. If the exchange name is specified, and that
			/// exchange does not exist, the server will raise a channel exception.
			/// </summary>
			public ExchangeName Exchange
			{
				get => _exchange;
				set
				{
					_exchange = value;
				}
			}

			private Shortstr _routingKey;
			/// <summary>
			/// Specifies the routing key for the message. The routing key is used for routing
			/// messages depending on the exchange configuration.
			/// </summary>
			public Shortstr RoutingKey
			{
				get => _routingKey;
				set
				{
					_routingKey = value;
				}
			}

			private Bit _mandatory;
			/// <summary>
			/// This flag tells the server how to react if the message cannot be routed to a
			/// queue. If this flag is set, the server will return an unroutable message with a
			/// Return method. If this flag is zero, the server silently drops the message.
			/// </summary>
			public Bit Mandatory
			{
				get => _mandatory;
				set
				{
					_mandatory = value;
				}
			}

			private Bit _immediate;
			/// <summary>
			/// This flag tells the server how to react if the message cannot be routed to a
			/// queue consumer immediately. If this flag is set, the server will return an
			/// undeliverable message with a Return method. If this flag is zero, the server
			/// will queue the message, but with no guarantee that it will ever be consumed.
			/// </summary>
			public Bit Immediate
			{
				get => _immediate;
				set
				{
					_immediate = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_exchange = new ExchangeName(reader.ReadShortString());
				_routingKey = new Shortstr(reader.ReadShortString());
				_mandatory = new Bit(reader.ReadBit());
				_immediate = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_exchange.Value);
				writer.WriteShortString(_routingKey.Value);
				writer.WriteBit(_mandatory.Value);
				writer.WriteBit(_immediate.Value);
			}
		}

		/// <summary>
		/// This method returns an undeliverable message that was published with the "immediate"
		/// flag set, or an unroutable message published with the "mandatory" flag set. The
		/// reply code and text provide information about the reason that the message was
		/// undeliverable.
		/// </summary>
		public class Return : IMethod, IContentMethod<Basic.ContentHeader>, IServerMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 50;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Basic.ContentHeader ContentHeader { get; private set; }

			public void SetContentHeader(IContentHeader contentHeader)
			{
				if (typeof(Basic.ContentHeader) != contentHeader.GetType())
				{
					throw new UnexpectedFrameException($"Did not expect {contentHeader.GetType()}, expected {typeof(Basic.ContentHeader)}.");
				}
				ContentHeader = (Basic.ContentHeader) contentHeader;
			}

			public byte[] ContentBody => _contentBodyFragments.SelectMany(fragment => fragment.Payload).ToArray();

			public IContentBody[] ContentBodyFragments => _contentBodyFragments.ToArray();

			private readonly List<IContentBody> _contentBodyFragments = new List<IContentBody>();
			public void AddContentBody(IContentBody contentBody)
			{
				_contentBodyFragments.Add(contentBody);
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private ReplyCode _replyCode;
			public ReplyCode ReplyCode
			{
				get => _replyCode;
				set
				{
					_replyCode = value;
				}
			}

			private ReplyText _replyText;
			public ReplyText ReplyText
			{
				get => _replyText;
				set
				{
					_replyText = value;
				}
			}

			private ExchangeName _exchange;
			/// <summary>
			/// Specifies the name of the exchange that the message was originally published
			/// to.  May be empty, meaning the default exchange.
			/// </summary>
			public ExchangeName Exchange
			{
				get => _exchange;
				set
				{
					_exchange = value;
				}
			}

			private Shortstr _routingKey;
			/// <summary>
			/// Specifies the routing key name specified when the message was published.
			/// </summary>
			public Shortstr RoutingKey
			{
				get => _routingKey;
				set
				{
					_routingKey = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_replyCode = new ReplyCode(reader.ReadShortInteger());
				_replyText = new ReplyText(reader.ReadShortString());
				_exchange = new ExchangeName(reader.ReadShortString());
				_routingKey = new Shortstr(reader.ReadShortString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_replyCode.Value);
				writer.WriteShortString(_replyText.Value);
				writer.WriteShortString(_exchange.Value);
				writer.WriteShortString(_routingKey.Value);
			}
		}

		/// <summary>
		/// This method delivers a message to the client, via a consumer. In the asynchronous
		/// message delivery model, the client starts a consumer using the Consume method, then
		/// the server responds with Deliver methods as and when messages arrive for that
		/// consumer.
		/// </summary>
		public class Deliver : IMethod, IContentMethod<Basic.ContentHeader>, IServerMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 60;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Basic.ContentHeader ContentHeader { get; private set; }

			public void SetContentHeader(IContentHeader contentHeader)
			{
				if (typeof(Basic.ContentHeader) != contentHeader.GetType())
				{
					throw new UnexpectedFrameException($"Did not expect {contentHeader.GetType()}, expected {typeof(Basic.ContentHeader)}.");
				}
				ContentHeader = (Basic.ContentHeader) contentHeader;
			}

			public byte[] ContentBody => _contentBodyFragments.SelectMany(fragment => fragment.Payload).ToArray();

			public IContentBody[] ContentBodyFragments => _contentBodyFragments.ToArray();

			private readonly List<IContentBody> _contentBodyFragments = new List<IContentBody>();
			public void AddContentBody(IContentBody contentBody)
			{
				_contentBodyFragments.Add(contentBody);
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private ConsumerTag _consumerTag;
			public ConsumerTag ConsumerTag
			{
				get => _consumerTag;
				set
				{
					_consumerTag = value;
				}
			}

			private DeliveryTag _deliveryTag;
			public DeliveryTag DeliveryTag
			{
				get => _deliveryTag;
				set
				{
					_deliveryTag = value;
				}
			}

			private Redelivered _redelivered;
			public Redelivered Redelivered
			{
				get => _redelivered;
				set
				{
					_redelivered = value;
				}
			}

			private ExchangeName _exchange;
			/// <summary>
			/// Specifies the name of the exchange that the message was originally published to.
			/// May be empty, indicating the default exchange.
			/// </summary>
			public ExchangeName Exchange
			{
				get => _exchange;
				set
				{
					_exchange = value;
				}
			}

			private Shortstr _routingKey;
			/// <summary>
			/// Specifies the routing key name specified when the message was published.
			/// </summary>
			public Shortstr RoutingKey
			{
				get => _routingKey;
				set
				{
					_routingKey = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_consumerTag = new ConsumerTag(reader.ReadShortString());
				_deliveryTag = new DeliveryTag(reader.ReadLongLongInteger());
				_redelivered = new Redelivered(reader.ReadBit());
				_exchange = new ExchangeName(reader.ReadShortString());
				_routingKey = new Shortstr(reader.ReadShortString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_consumerTag.Value);
				writer.WriteLongLongInteger(_deliveryTag.Value);
				writer.WriteBit(_redelivered.Value);
				writer.WriteShortString(_exchange.Value);
				writer.WriteShortString(_routingKey.Value);
			}
		}

		/// <summary>
		/// This method provides a direct access to the messages in a queue using a synchronous
		/// dialogue that is designed for specific types of application where synchronous
		/// functionality is more important than performance.
		/// </summary>
		public class Get : IMethod, INonContentMethod, IRespond<GetOk>, IRespond<GetEmpty>, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 70;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public GetOk Respond(GetOk method) 
			{
				return method;
			}


			public GetEmpty Respond(GetEmpty method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(GetOk), typeof(GetEmpty) };
			}

			private Short _reserved1;
			public Short Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			private QueueName _queue;
			/// <summary>
			/// Specifies the name of the queue to get a message from.
			/// </summary>
			public QueueName Queue
			{
				get => _queue;
				set
				{
					_queue = value;
				}
			}

			private NoAck _noAck;
			public NoAck NoAck
			{
				get => _noAck;
				set
				{
					_noAck = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Short(reader.ReadShortInteger());
				_queue = new QueueName(reader.ReadShortString());
				_noAck = new NoAck(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortInteger(_reserved1.Value);
				writer.WriteShortString(_queue.Value);
				writer.WriteBit(_noAck.Value);
			}
		}

		/// <summary>
		/// This method delivers a message to the client following a get method. A message
		/// delivered by 'get-ok' must be acknowledged unless the no-ack option was set in the
		/// get method.
		/// </summary>
		public class GetOk : IMethod, IContentMethod<Basic.ContentHeader>, IServerMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 71;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Basic.ContentHeader ContentHeader { get; private set; }

			public void SetContentHeader(IContentHeader contentHeader)
			{
				if (typeof(Basic.ContentHeader) != contentHeader.GetType())
				{
					throw new UnexpectedFrameException($"Did not expect {contentHeader.GetType()}, expected {typeof(Basic.ContentHeader)}.");
				}
				ContentHeader = (Basic.ContentHeader) contentHeader;
			}

			public byte[] ContentBody => _contentBodyFragments.SelectMany(fragment => fragment.Payload).ToArray();

			public IContentBody[] ContentBodyFragments => _contentBodyFragments.ToArray();

			private readonly List<IContentBody> _contentBodyFragments = new List<IContentBody>();
			public void AddContentBody(IContentBody contentBody)
			{
				_contentBodyFragments.Add(contentBody);
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private DeliveryTag _deliveryTag;
			public DeliveryTag DeliveryTag
			{
				get => _deliveryTag;
				set
				{
					_deliveryTag = value;
				}
			}

			private Redelivered _redelivered;
			public Redelivered Redelivered
			{
				get => _redelivered;
				set
				{
					_redelivered = value;
				}
			}

			private ExchangeName _exchange;
			/// <summary>
			/// Specifies the name of the exchange that the message was originally published to.
			/// If empty, the message was published to the default exchange.
			/// </summary>
			public ExchangeName Exchange
			{
				get => _exchange;
				set
				{
					_exchange = value;
				}
			}

			private Shortstr _routingKey;
			/// <summary>
			/// Specifies the routing key name specified when the message was published.
			/// </summary>
			public Shortstr RoutingKey
			{
				get => _routingKey;
				set
				{
					_routingKey = value;
				}
			}

			private MessageCount _messageCount;
			public MessageCount MessageCount
			{
				get => _messageCount;
				set
				{
					_messageCount = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_deliveryTag = new DeliveryTag(reader.ReadLongLongInteger());
				_redelivered = new Redelivered(reader.ReadBit());
				_exchange = new ExchangeName(reader.ReadShortString());
				_routingKey = new Shortstr(reader.ReadShortString());
				_messageCount = new MessageCount(reader.ReadLongInteger());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongLongInteger(_deliveryTag.Value);
				writer.WriteBit(_redelivered.Value);
				writer.WriteShortString(_exchange.Value);
				writer.WriteShortString(_routingKey.Value);
				writer.WriteLongInteger(_messageCount.Value);
			}
		}

		/// <summary>
		/// This method tells the client that the queue has no messages available for the
		/// client.
		/// </summary>
		public class GetEmpty : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 72;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Shortstr _reserved1;
			public Shortstr Reserved1
			{
				get => _reserved1;
				set
				{
					_reserved1 = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_reserved1 = new Shortstr(reader.ReadShortString());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteShortString(_reserved1.Value);
			}
		}

		/// <summary>
		/// This method acknowledges one or more messages delivered via the Deliver or Get-Ok
		/// methods. The client can ask to confirm a single message or a set of messages up to
		/// and including a specific message.
		/// </summary>
		public class Ack : IMethod, INonContentMethod, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 80;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private DeliveryTag _deliveryTag;
			public DeliveryTag DeliveryTag
			{
				get => _deliveryTag;
				set
				{
					_deliveryTag = value;
				}
			}

			private Bit _multiple;
			/// <summary>
			/// If set to 1, the delivery tag is treated as "up to and including", so that the
			/// client can acknowledge multiple messages with a single method. If set to zero,
			/// the delivery tag refers to a single message. If the multiple field is 1, and the
			/// delivery tag is zero, tells the server to acknowledge all outstanding messages.
			/// </summary>
			public Bit Multiple
			{
				get => _multiple;
				set
				{
					_multiple = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_deliveryTag = new DeliveryTag(reader.ReadLongLongInteger());
				_multiple = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongLongInteger(_deliveryTag.Value);
				writer.WriteBit(_multiple.Value);
			}
		}

		/// <summary>
		/// This method allows a client to reject a message. It can be used to interrupt and
		/// cancel large incoming messages, or return untreatable messages to their original
		/// queue.
		/// </summary>
		public class Reject : IMethod, INonContentMethod, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 90;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private DeliveryTag _deliveryTag;
			public DeliveryTag DeliveryTag
			{
				get => _deliveryTag;
				set
				{
					_deliveryTag = value;
				}
			}

			private Bit _requeue;
			/// <summary>
			/// If requeue is true, the server will attempt to requeue the message.  If requeue
			/// is false or the requeue  attempt fails the messages are discarded or dead-lettered.
			/// </summary>
			public Bit Requeue
			{
				get => _requeue;
				set
				{
					_requeue = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_deliveryTag = new DeliveryTag(reader.ReadLongLongInteger());
				_requeue = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteLongLongInteger(_deliveryTag.Value);
				writer.WriteBit(_requeue.Value);
			}
		}

		/// <summary>
		/// This method asks the server to redeliver all unacknowledged messages on a
		/// specified channel. Zero or more messages may be redelivered.  This method
		/// is deprecated in favour of the synchronous Recover/Recover-Ok.
		/// </summary>
		public class RecoverAsync : IMethod, INonContentMethod, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 100;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Bit _requeue;
			/// <summary>
			/// If this field is zero, the message will be redelivered to the original
			/// recipient. If this bit is 1, the server will attempt to requeue the message,
			/// potentially then delivering it to an alternative subscriber.
			/// </summary>
			public Bit Requeue
			{
				get => _requeue;
				set
				{
					_requeue = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_requeue = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteBit(_requeue.Value);
			}
		}

		/// <summary>
		/// This method asks the server to redeliver all unacknowledged messages on a
		/// specified channel. Zero or more messages may be redelivered.  This method
		/// replaces the asynchronous Recover.
		/// </summary>
		public class Recover : IMethod, INonContentMethod, IClientMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 110;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			private Bit _requeue;
			/// <summary>
			/// If this field is zero, the message will be redelivered to the original
			/// recipient. If this bit is 1, the server will attempt to requeue the message,
			/// potentially then delivering it to an alternative subscriber.
			/// </summary>
			public Bit Requeue
			{
				get => _requeue;
				set
				{
					_requeue = value;
				}
			}

			public void ReadFrom(IAmqpReader reader)
			{
				_requeue = new Bit(reader.ReadBit());
			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);

				writer.WriteBit(_requeue.Value);
			}
		}

		/// <summary>
		/// This method acknowledges a Basic.Recover method.
		/// </summary>
		public class RecoverOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 60;
			public int ProtocolMethodId => 111;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}
	}

	/// <summary>
	/// The Tx class allows publish and ack operations to be batched into atomic
	/// units of work.  The intention is that all publish and ack requests issued
	/// within a transaction will complete successfully or none of them will.
	/// Servers SHOULD implement atomic transactions at least where all publish
	/// or ack requests affect a single queue.  Transactions that cover multiple
	/// queues may be non-atomic, given that queues can be created and destroyed
	/// asynchronously, and such events do not form part of any transaction.
	/// Further, the behaviour of transactions with respect to the immediate and
	/// mandatory flags on Basic.Publish methods is not defined.
	/// </summary>
	/// <example>
	/// 
	///       tx                  = C:SELECT S:SELECT-OK
	///                           / C:COMMIT S:COMMIT-OK
	///                           / C:ROLLBACK S:ROLLBACK-OK
	/// </example>
	public class Tx
	{
		/// <summary>
		/// This method sets the channel to use standard transactions. The client must use this
		/// method at least once on a channel before using the Commit or Rollback methods.
		/// </summary>
		public class Select : IMethod, INonContentMethod, IRespond<SelectOk>, IClientMethod
		{
			public int ProtocolClassId => 90;
			public int ProtocolMethodId => 10;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public SelectOk Respond(SelectOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(SelectOk) };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method confirms to the client that the channel was successfully set to use
		/// standard transactions.
		/// </summary>
		public class SelectOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 90;
			public int ProtocolMethodId => 11;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method commits all message publications and acknowledgments performed in
		/// the current transaction.  A new transaction starts immediately after a commit.
		/// </summary>
		public class Commit : IMethod, INonContentMethod, IRespond<CommitOk>, IClientMethod
		{
			public int ProtocolClassId => 90;
			public int ProtocolMethodId => 20;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public CommitOk Respond(CommitOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(CommitOk) };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method confirms to the client that the commit succeeded. Note that if a commit
		/// fails, the server raises a channel exception.
		/// </summary>
		public class CommitOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 90;
			public int ProtocolMethodId => 21;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method abandons all message publications and acknowledgments performed in
		/// the current transaction. A new transaction starts immediately after a rollback.
		/// Note that unacked messages will not be automatically redelivered by rollback;
		/// if that is required an explicit recover call should be issued.
		/// </summary>
		public class Rollback : IMethod, INonContentMethod, IRespond<RollbackOk>, IClientMethod
		{
			public int ProtocolClassId => 90;
			public int ProtocolMethodId => 30;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public RollbackOk Respond(RollbackOk method) 
			{
				return method;
			}

			public Type[] Responses() 
			{
				return new Type[] { typeof(RollbackOk) };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}

		/// <summary>
		/// This method confirms to the client that the rollback succeeded. Note that if an
		/// rollback fails, the server raises a channel exception.
		/// </summary>
		public class RollbackOk : IMethod, INonContentMethod, IServerMethod
		{
			public int ProtocolClassId => 90;
			public int ProtocolMethodId => 31;

			public bool SentOnValidChannel(int channel)
			{
				return channel > 0;
			}

			public Type[] Responses() 
			{
				return new Type[] {  };
			}

			public void ReadFrom(IAmqpReader reader)
			{

			}

			public void WriteTo(IAmqpWriter writer)
			{
				writer.WriteShortUnsignedInteger((ushort)ProtocolClassId);
				writer.WriteShortUnsignedInteger((ushort)ProtocolMethodId);


			}
		}
	}

	public class ProtocolHeader : IProtocolHeader, IRespond<Connection.Start>
	{
		private readonly ILogger _logger = LogFactory.Create<Amq091Protocol>();

		public string Protocol { get; } = "AMQP";
		public IVersion Version { get; private set; } 
		private const byte ProtocolId = 0;

		public void WriteTo(IAmqpWriter writer)
		{
			writer.WriteBytes(Encoding.UTF8.GetBytes(Protocol));
			writer.WriteByte(ProtocolId);
			writer.WriteByte((byte)Version.Major);
			writer.WriteByte((byte)Version.Minor);
			writer.WriteByte((byte)Version.Revision);
		}

		public void ReadFrom(IAmqpReader reader)
		{
			var protocol = Encoding.UTF8.GetString(reader.ReadBytes(4));

			var constant = reader.ReadByte();

			Version = new ProtocolHeaderVersion(reader);

			if (protocol != Protocol || constant != ProtocolId)
			{
				IsValid = false;
				_logger.Error($"Incorrect header. Expected '{Protocol}{ProtocolId}<major version><minor version><revision>'. Got '{protocol}{constant}{Version.Major}{Version.Minor}{Version.Revision}'.");
			}
		}

		public bool IsValid { get; private set; } = true;

		private class ProtocolHeaderVersion : IVersion
		{
			public ProtocolHeaderVersion(IAmqpReader reader)
			{
				Major = reader.ReadByte();
				Minor = reader.ReadByte();
				Revision = reader.ReadByte();
			}

			public int Major { get; }
			public int Minor { get; }
			public int Revision { get; }
		}

		public Connection.Start Respond(Connection.Start method)
		{
			return method;
		}

		public ProtocolHeader Respond(ProtocolHeader protocolHeader)
		{
			return protocolHeader;
		}
	}

	public class ContentBody : IContentBody
	{
		public byte[] Payload { get; private set; }

		public bool SentOnValidChannel(int channel)
		{
			return channel > 0;
		}

		public void ReadFrom(IAmqpReader reader)
		{
			Payload = reader.ReadBytes(reader.Length);
		}

		public void WriteTo(IAmqpWriter writer)
		{
			writer.WriteBytes(Payload);

		}
	}

	public class Heartbeat : IHeartbeat
	{
		public bool SentOnValidChannel(int channel)
		{
			return channel == 0;
		}

		public void ReadFrom(IAmqpReader reader)
		{

		}

		public void WriteTo(IAmqpWriter writer)
		{

		}
	}
}