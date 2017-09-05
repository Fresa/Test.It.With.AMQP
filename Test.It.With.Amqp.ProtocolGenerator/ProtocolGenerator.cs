// WARNING! THIS FILE IS AUTO-GENERATED! DO NOT EDIT.

using Validation;
using System.Text.RegularExpressions;
using Test.It.With.RabbitMQ.MessageClient;

namespace Test.It.With.RabbitMQ
{
	internal class Constants
	{
		private const int FrameMethod = 1;

		private const int FrameHeader = 2;

		private const int FrameBody = 3;

		private const int FrameHeartbeat = 8;

		private const int FrameMinSize = 4096;

		private const int FrameEnd = 206;

		/// <summary>
		/// Indicates that the method completed successfully. This reply code is
		/// reserved for future use - the current protocol design does not use positive
		/// confirmation and reply codes are sent only in case of an error.
		/// </summary>
		private const int ReplySuccess = 200;

		/// <summary>
		/// The client attempted to transfer content larger than the server could accept
		/// at the present time. The client may retry at a later time.
		/// </summary>
		private const int ContentTooLarge = 311;

		/// <summary>
		/// When the exchange cannot deliver to a consumer when the immediate flag is
		/// set. As a result of pending data on the queue or the absence of any
		/// consumers of the queue.
		/// </summary>
		private const int NoConsumers = 313;

		/// <summary>
		/// An operator intervened to close the connection for some reason. The client
		/// may retry at some later date.
		/// </summary>
		private const int ConnectionForced = 320;

		/// <summary>
		/// The client tried to work with an unknown virtual host.
		/// </summary>
		private const int InvalidPath = 402;

		/// <summary>
		/// The client attempted to work with a server entity to which it has no
		/// access due to security settings.
		/// </summary>
		private const int AccessRefused = 403;

		/// <summary>
		/// The client attempted to work with a server entity that does not exist.
		/// </summary>
		private const int NotFound = 404;

		/// <summary>
		/// The client attempted to work with a server entity to which it has no
		/// access because another client is working with it.
		/// </summary>
		private const int ResourceLocked = 405;

		/// <summary>
		/// The client requested a method that was not allowed because some precondition
		/// failed.
		/// </summary>
		private const int PreconditionFailed = 406;

		/// <summary>
		/// The sender sent a malformed frame that the recipient could not decode.
		/// This strongly implies a programming error in the sending peer.
		/// </summary>
		private const int FrameError = 501;

		/// <summary>
		/// The sender sent a frame that contained illegal values for one or more
		/// fields. This strongly implies a programming error in the sending peer.
		/// </summary>
		private const int SyntaxError = 502;

		/// <summary>
		/// The client sent an invalid sequence of frames, attempting to perform an
		/// operation that was considered invalid by the server. This usually implies
		/// a programming error in the client.
		/// </summary>
		private const int CommandInvalid = 503;

		/// <summary>
		/// The client attempted to work with a channel that had not been correctly
		/// opened. This most likely indicates a fault in the client layer.
		/// </summary>
		private const int ChannelError = 504;

		/// <summary>
		/// The peer sent a frame that was not expected, usually in the context of
		/// a content header and body.  This strongly indicates a fault in the peer's
		/// content processing.
		/// </summary>
		private const int UnexpectedFrame = 505;

		/// <summary>
		/// The server could not complete the method because it lacked sufficient
		/// resources. This may be due to the client creating too many of some type
		/// of entity.
		/// </summary>
		private const int ResourceError = 506;

		/// <summary>
		/// The client tried to work with some entity in a manner that is prohibited
		/// by the server, due to security settings or by some other criteria.
		/// </summary>
		private const int NotAllowed = 530;

		/// <summary>
		/// The client tried to use functionality that is not implemented in the
		/// server.
		/// </summary>
		private const int NotImplemented = 540;

		/// <summary>
		/// The server could not complete the method because of an internal error.
		/// The server may require intervention by an operator in order to resume
		/// normal operations.
		/// </summary>
		private const int InternalError = 541;
	}

	public struct ClassId 
	{
		public System.Int16 Value { get; }

		internal ClassId(System.Int16 value)
		{
			Value = value;
		}
	}

	/// <summary>
	/// Identifier for the consumer, valid within the current channel.
	/// </summary>
	public struct ConsumerTag 
	{
		public System.String Value { get; }

		internal ConsumerTag(System.String value)
		{
			Value = value;
		}
	}

	/// <summary>
	/// The server-assigned and channel-specific delivery tag
	/// </summary>
	public struct DeliveryTag 
	{
		public System.Int64 Value { get; }

		internal DeliveryTag(System.Int64 value)
		{
			Value = value;
		}
	}

	/// <summary>
	/// The exchange name is a client-selected string that identifies the exchange for
	/// publish methods.
	/// </summary>
	public struct ExchangeName 
	{
		public System.String Value { get; }

		internal ExchangeName(System.String value)
		{
			Requires.Range(value.Length <= 127, nameof(value));
			Requires.That(Regex.IsMatch(value, "^[a-zA-Z0-9-_.:]*$"), nameof(value), "Value must meet the following regex criteria: ^[a-zA-Z0-9-_.:]*$");Value = value;
		}
	}

	public struct MethodId 
	{
		public System.Int16 Value { get; }

		internal MethodId(System.Int16 value)
		{
			Value = value;
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

		internal NoAck(System.Boolean value)
		{
			Value = value;
		}
	}

	/// <summary>
	/// If the no-local field is set the server will not send messages to the connection that
	/// published them.
	/// </summary>
	public struct NoLocal 
	{
		public System.Boolean Value { get; }

		internal NoLocal(System.Boolean value)
		{
			Value = value;
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

		internal NoWait(System.Boolean value)
		{
			Value = value;
		}
	}

	/// <summary>
	/// Unconstrained.
	/// </summary>
	public struct Path 
	{
		public System.String Value { get; }

		internal Path(System.String value)
		{
			Requires.NotNullAllowStructs(value, nameof(value));
			Requires.Range(value.Length <= 127, nameof(value));Value = value;
		}
	}

	/// <summary>
	/// This table provides a set of peer properties, used for identification, debugging,
	/// and general information.
	/// </summary>
	public struct PeerProperties 
	{
		public System.Collections.Generic.IDictionary<System.String, System.Object> Value { get; }

		internal PeerProperties(System.Collections.Generic.IDictionary<System.String, System.Object> value)
		{
			Value = value;
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

		internal QueueName(System.String value)
		{
			Requires.Range(value.Length <= 127, nameof(value));
			Requires.That(Regex.IsMatch(value, "^[a-zA-Z0-9-_.:]*$"), nameof(value), "Value must meet the following regex criteria: ^[a-zA-Z0-9-_.:]*$");Value = value;
		}
	}

	/// <summary>
	/// This indicates that the message has been previously delivered to this or
	/// another client.
	/// </summary>
	public struct Redelivered 
	{
		public System.Boolean Value { get; }

		internal Redelivered(System.Boolean value)
		{
			Value = value;
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

		internal MessageCount(System.Int32 value)
		{
			Value = value;
		}
	}

	/// <summary>
	/// The reply code. The AMQ reply codes are defined as constants at the start
	/// of this formal specification.
	/// </summary>
	public struct ReplyCode 
	{
		public System.Int16 Value { get; }

		internal ReplyCode(System.Int16 value)
		{
			Requires.NotNullAllowStructs(value, nameof(value));Value = value;
		}
	}

	/// <summary>
	/// The localised reply text. This text can be logged as an aid to resolving
	/// issues.
	/// </summary>
	public struct ReplyText 
	{
		public System.String Value { get; }

		internal ReplyText(System.String value)
		{
			Requires.NotNullAllowStructs(value, nameof(value));Value = value;
		}
	}

	public struct Bit 
	{
		public System.Boolean Value { get; }

		internal Bit(System.Boolean value)
		{
			Value = value;
		}
	}

	public struct Octet 
	{
		public System.Byte Value { get; }

		internal Octet(System.Byte value)
		{
			Value = value;
		}
	}

	public struct Short 
	{
		public System.Int16 Value { get; }

		internal Short(System.Int16 value)
		{
			Value = value;
		}
	}

	public struct Long 
	{
		public System.Int32 Value { get; }

		internal Long(System.Int32 value)
		{
			Value = value;
		}
	}

	public struct Longlong 
	{
		public System.Int64 Value { get; }

		internal Longlong(System.Int64 value)
		{
			Value = value;
		}
	}

	public struct Shortstr 
	{
		public System.String Value { get; }

		internal Shortstr(System.String value)
		{
			Value = value;
		}
	}

	public struct Longstr 
	{
		public System.Byte[] Value { get; }

		internal Longstr(System.Byte[] value)
		{
			Value = value;
		}
	}

	public struct Timestamp 
	{
		public System.DateTime Value { get; }

		internal Timestamp(System.DateTime value)
		{
			Value = value;
		}
	}

	public struct Table 
	{
		public System.Collections.Generic.IDictionary<System.String, System.Object> Value { get; }

		internal Table(System.Collections.Generic.IDictionary<System.String, System.Object> value)
		{
			Value = value;
		}
	}

	/// <summary>
	/// This method starts the connection negotiation process by telling the client the
	/// protocol version that the server proposes, along with a list of security mechanisms
	/// which the client can use for authentication.
	/// </summary>
	public class ConnectionStart
	{
		public const int ClassId = 10;
		public const int MethodId = 10;
		/// <summary>
		/// The major version number can take any value from 0 to 99 as defined in the
		/// AMQP specification.
		/// </summary>
		private Octet _versionmajor;
		public Octet VersionMajor_
		{
			get
			{
				return _versionmajor;
			}
			set
			{
				_versionmajor = value;
			}
		}

		/// <summary>
		/// The minor version number can take any value from 0 to 99 as defined in the
		/// AMQP specification.
		/// </summary>
		private Octet _versionminor;
		public Octet VersionMinor_
		{
			get
			{
				return _versionminor;
			}
			set
			{
				_versionminor = value;
			}
		}

		private PeerProperties _serverproperties;
		public PeerProperties ServerProperties_
		{
			get
			{
				return _serverproperties;
			}
			set
			{
				_serverproperties = value;
			}
		}

		/// <summary>
		/// A list of the security mechanisms that the server supports, delimited by spaces.
		/// </summary>
		private Longstr _mechanisms;
		public Longstr Mechanisms_
		{
			get
			{
				return _mechanisms;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_mechanisms = value;
			}
		}

		/// <summary>
		/// A list of the message locales that the server supports, delimited by spaces. The
		/// locale defines the language in which the server will send reply texts.
		/// </summary>
		private Longstr _locales;
		public Longstr Locales_
		{
			get
			{
				return _locales;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_locales = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_versionmajor = new Octet(reader.ReadByte());
			_versionminor = new Octet(reader.ReadByte());
			_serverproperties = new PeerProperties(reader.ReadTable());
			_mechanisms = new Longstr(reader.ReadLongString());
			_locales = new Longstr(reader.ReadLongString());
		}
	}

	/// <summary>
	/// This method selects a SASL security mechanism.
	/// </summary>
	public class ConnectionStartOk
	{
		public const int ClassId = 10;
		public const int MethodId = 11;
		private PeerProperties _clientproperties;
		public PeerProperties ClientProperties_
		{
			get
			{
				return _clientproperties;
			}
			set
			{
				_clientproperties = value;
			}
		}

		/// <summary>
		/// A single security mechanisms selected by the client, which must be one of those
		/// specified by the server.
		/// </summary>
		private Shortstr _mechanism;
		public Shortstr Mechanism_
		{
			get
			{
				return _mechanism;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_mechanism = value;
			}
		}

		/// <summary>
		/// A block of opaque data passed to the security mechanism. The contents of this
		/// data are defined by the SASL security mechanism.
		/// </summary>
		private Longstr _response;
		public Longstr Response_
		{
			get
			{
				return _response;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_response = value;
			}
		}

		/// <summary>
		/// A single message locale selected by the client, which must be one of those
		/// specified by the server.
		/// </summary>
		private Shortstr _locale;
		public Shortstr Locale_
		{
			get
			{
				return _locale;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_locale = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_clientproperties = new PeerProperties(reader.ReadTable());
			_mechanism = new Shortstr(reader.ReadShortString());
			_response = new Longstr(reader.ReadLongString());
			_locale = new Shortstr(reader.ReadShortString());
		}
	}

	/// <summary>
	/// The SASL protocol works by exchanging challenges and responses until both peers have
	/// received sufficient information to authenticate each other. This method challenges
	/// the client to provide more information.
	/// </summary>
	public class ConnectionSecure
	{
		public const int ClassId = 10;
		public const int MethodId = 20;
		/// <summary>
		/// Challenge information, a block of opaque binary data passed to the security
		/// mechanism.
		/// </summary>
		private Longstr _challenge;
		public Longstr Challenge_
		{
			get
			{
				return _challenge;
			}
			set
			{
				_challenge = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_challenge = new Longstr(reader.ReadLongString());
		}
	}

	/// <summary>
	/// This method attempts to authenticate, passing a block of SASL data for the security
	/// mechanism at the server side.
	/// </summary>
	public class ConnectionSecureOk
	{
		public const int ClassId = 10;
		public const int MethodId = 21;
		/// <summary>
		/// A block of opaque data passed to the security mechanism. The contents of this
		/// data are defined by the SASL security mechanism.
		/// </summary>
		private Longstr _response;
		public Longstr Response_
		{
			get
			{
				return _response;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_response = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_response = new Longstr(reader.ReadLongString());
		}
	}

	/// <summary>
	/// This method proposes a set of connection configuration values to the client. The
	/// client can accept and/or adjust these.
	/// </summary>
	public class ConnectionTune
	{
		public const int ClassId = 10;
		public const int MethodId = 30;
		/// <summary>
		/// Specifies highest channel number that the server permits.  Usable channel numbers
		/// are in the range 1..channel-max.  Zero indicates no specified limit.
		/// </summary>
		private Short _channelmax;
		public Short ChannelMax_
		{
			get
			{
				return _channelmax;
			}
			set
			{
				_channelmax = value;
			}
		}

		/// <summary>
		/// The largest frame size that the server proposes for the connection, including
		/// frame header and end-byte.  The client can negotiate a lower value. Zero means
		/// that the server does not impose any specific limit but may reject very large
		/// frames if it cannot allocate resources for them.
		/// </summary>
		private Long _framemax;
		public Long FrameMax_
		{
			get
			{
				return _framemax;
			}
			set
			{
				_framemax = value;
			}
		}

		/// <summary>
		/// The delay, in seconds, of the connection heartbeat that the server wants.
		/// Zero means the server does not want a heartbeat.
		/// </summary>
		private Short _heartbeat;
		public Short Heartbeat_
		{
			get
			{
				return _heartbeat;
			}
			set
			{
				_heartbeat = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_channelmax = new Short(reader.ReadShortInteger());
			_framemax = new Long(reader.ReadLongInteger());
			_heartbeat = new Short(reader.ReadShortInteger());
		}
	}

	/// <summary>
	/// This method sends the client's connection tuning parameters to the server.
	/// Certain fields are negotiated, others provide capability information.
	/// </summary>
	public class ConnectionTuneOk
	{
		public const int ClassId = 10;
		public const int MethodId = 31;
		/// <summary>
		/// The maximum total number of channels that the client will use per connection.
		/// </summary>
		private Short _channelmax;
		public Short ChannelMax_
		{
			get
			{
				return _channelmax;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));

				_channelmax = value;
			}
		}

		/// <summary>
		/// The largest frame size that the client and server will use for the connection.
		/// Zero means that the client does not impose any specific limit but may reject
		/// very large frames if it cannot allocate resources for them. Note that the
		/// frame-max limit applies principally to content frames, where large contents can
		/// be broken into frames of arbitrary size.
		/// </summary>
		private Long _framemax;
		public Long FrameMax_
		{
			get
			{
				return _framemax;
			}
			set
			{
				_framemax = value;
			}
		}

		/// <summary>
		/// The delay, in seconds, of the connection heartbeat that the client wants. Zero
		/// means the client does not want a heartbeat.
		/// </summary>
		private Short _heartbeat;
		public Short Heartbeat_
		{
			get
			{
				return _heartbeat;
			}
			set
			{
				_heartbeat = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_channelmax = new Short(reader.ReadShortInteger());
			_framemax = new Long(reader.ReadLongInteger());
			_heartbeat = new Short(reader.ReadShortInteger());
		}
	}

	/// <summary>
	/// This method opens a connection to a virtual host, which is a collection of
	/// resources, and acts to separate multiple application domains within a server.
	/// The server may apply arbitrary limits per virtual host, such as the number
	/// of each type of entity that may be used, per connection and/or in total.
	/// </summary>
	public class ConnectionOpen
	{
		public const int ClassId = 10;
		public const int MethodId = 40;
		/// <summary>
		/// The name of the virtual host to work with.
		/// </summary>
		private Path _virtualhost;
		public Path VirtualHost_
		{
			get
			{
				return _virtualhost;
			}
			set
			{
				_virtualhost = value;
			}
		}

		private Shortstr _reserved1;
		public Shortstr Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		private Bit _reserved2;
		public Bit Reserved2_
		{
			get
			{
				return _reserved2;
			}
			set
			{
				_reserved2 = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_virtualhost = new Path(reader.ReadShortString());
			_reserved1 = new Shortstr(reader.ReadShortString());
			_reserved2 = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method signals to the client that the connection is ready for use.
	/// </summary>
	public class ConnectionOpenOk
	{
		public const int ClassId = 10;
		public const int MethodId = 41;
		private Shortstr _reserved1;
		public Shortstr Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Shortstr(reader.ReadShortString());
		}
	}

	/// <summary>
	/// This method indicates that the sender wants to close the connection. This may be
	/// due to internal conditions (e.g. a forced shut-down) or due to an error handling
	/// a specific method, i.e. an exception. When a close is due to an exception, the
	/// sender provides the class and method id of the method which caused the exception.
	/// </summary>
	public class ConnectionClose
	{
		public const int ClassId = 10;
		public const int MethodId = 50;
		private ReplyCode _replycode;
		public ReplyCode ReplyCode_
		{
			get
			{
				return _replycode;
			}
			set
			{
				_replycode = value;
			}
		}

		private ReplyText _replytext;
		public ReplyText ReplyText_
		{
			get
			{
				return _replytext;
			}
			set
			{
				_replytext = value;
			}
		}

		/// <summary>
		/// When the close is provoked by a method exception, this is the class of the
		/// method.
		/// </summary>
		private ClassId _classid;
		public ClassId ClassId_
		{
			get
			{
				return _classid;
			}
			set
			{
				_classid = value;
			}
		}

		/// <summary>
		/// When the close is provoked by a method exception, this is the ID of the method.
		/// </summary>
		private MethodId _methodid;
		public MethodId MethodId_
		{
			get
			{
				return _methodid;
			}
			set
			{
				_methodid = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_replycode = new ReplyCode(reader.ReadShortInteger());
			_replytext = new ReplyText(reader.ReadShortString());
			_classid = new ClassId(reader.ReadShortInteger());
			_methodid = new MethodId(reader.ReadShortInteger());
		}
	}

	/// <summary>
	/// This method confirms a Connection.Close method and tells the recipient that it is
	/// safe to release resources for the connection and close the socket.
	/// </summary>
	public class ConnectionCloseOk
	{
		public const int ClassId = 10;
		public const int MethodId = 51;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method opens a channel to the server.
	/// </summary>
	public class ChannelOpen
	{
		public const int ClassId = 20;
		public const int MethodId = 10;
		private Shortstr _reserved1;
		public Shortstr Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Shortstr(reader.ReadShortString());
		}
	}

	/// <summary>
	/// This method signals to the client that the channel is ready for use.
	/// </summary>
	public class ChannelOpenOk
	{
		public const int ClassId = 20;
		public const int MethodId = 11;
		private Longstr _reserved1;
		public Longstr Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Longstr(reader.ReadLongString());
		}
	}

	/// <summary>
	/// This method asks the peer to pause or restart the flow of content data sent by
	/// a consumer. This is a simple flow-control mechanism that a peer can use to avoid
	/// overflowing its queues or otherwise finding itself receiving more messages than
	/// it can process. Note that this method is not intended for window control. It does
	/// not affect contents returned by Basic.Get-Ok methods.
	/// </summary>
	public class ChannelFlow
	{
		public const int ClassId = 20;
		public const int MethodId = 20;
		/// <summary>
		/// If 1, the peer starts sending content frames. If 0, the peer stops sending
		/// content frames.
		/// </summary>
		private Bit _active;
		public Bit Active_
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_active = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// Confirms to the peer that a flow command was received and processed.
	/// </summary>
	public class ChannelFlowOk
	{
		public const int ClassId = 20;
		public const int MethodId = 21;
		/// <summary>
		/// Confirms the setting of the processed flow method: 1 means the peer will start
		/// sending or continue to send content frames; 0 means it will not.
		/// </summary>
		private Bit _active;
		public Bit Active_
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_active = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method indicates that the sender wants to close the channel. This may be due to
	/// internal conditions (e.g. a forced shut-down) or due to an error handling a specific
	/// method, i.e. an exception. When a close is due to an exception, the sender provides
	/// the class and method id of the method which caused the exception.
	/// </summary>
	public class ChannelClose
	{
		public const int ClassId = 20;
		public const int MethodId = 40;
		private ReplyCode _replycode;
		public ReplyCode ReplyCode_
		{
			get
			{
				return _replycode;
			}
			set
			{
				_replycode = value;
			}
		}

		private ReplyText _replytext;
		public ReplyText ReplyText_
		{
			get
			{
				return _replytext;
			}
			set
			{
				_replytext = value;
			}
		}

		/// <summary>
		/// When the close is provoked by a method exception, this is the class of the
		/// method.
		/// </summary>
		private ClassId _classid;
		public ClassId ClassId_
		{
			get
			{
				return _classid;
			}
			set
			{
				_classid = value;
			}
		}

		/// <summary>
		/// When the close is provoked by a method exception, this is the ID of the method.
		/// </summary>
		private MethodId _methodid;
		public MethodId MethodId_
		{
			get
			{
				return _methodid;
			}
			set
			{
				_methodid = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_replycode = new ReplyCode(reader.ReadShortInteger());
			_replytext = new ReplyText(reader.ReadShortString());
			_classid = new ClassId(reader.ReadShortInteger());
			_methodid = new MethodId(reader.ReadShortInteger());
		}
	}

	/// <summary>
	/// This method confirms a Channel.Close method and tells the recipient that it is safe
	/// to release resources for the channel.
	/// </summary>
	public class ChannelCloseOk
	{
		public const int ClassId = 20;
		public const int MethodId = 41;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method creates an exchange if it does not already exist, and if the exchange
	/// exists, verifies that it is of the correct and expected class.
	/// </summary>
	public class ExchangeDeclare
	{
		public const int ClassId = 40;
		public const int MethodId = 10;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		private ExchangeName _exchange;
		public ExchangeName Exchange_
		{
			get
			{
				return _exchange;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_exchange = value;
			}
		}

		/// <summary>
		/// Each exchange belongs to one of a set of exchange types implemented by the
		/// server. The exchange types define the functionality of the exchange - i.e. how
		/// messages are routed through it. It is not valid or meaningful to attempt to
		/// change the type of an existing exchange.
		/// </summary>
		private Shortstr _type;
		public Shortstr Type_
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// If set, the server will reply with Declare-Ok if the exchange already
		/// exists with the same name, and raise an error if not.  The client can
		/// use this to check whether an exchange exists without modifying the
		/// server state. When set, all other method fields except name and no-wait
		/// are ignored.  A declare with both passive and no-wait has no effect.
		/// Arguments are compared for semantic equivalence.
		/// </summary>
		private Bit _passive;
		public Bit Passive_
		{
			get
			{
				return _passive;
			}
			set
			{
				_passive = value;
			}
		}

		/// <summary>
		/// If set when creating a new exchange, the exchange will be marked as durable.
		/// Durable exchanges remain active when a server restarts. Non-durable exchanges
		/// (transient exchanges) are purged if/when a server restarts.
		/// </summary>
		private Bit _durable;
		public Bit Durable_
		{
			get
			{
				return _durable;
			}
			set
			{
				_durable = value;
			}
		}

		private Bit _reserved2;
		public Bit Reserved2_
		{
			get
			{
				return _reserved2;
			}
			set
			{
				_reserved2 = value;
			}
		}

		private Bit _reserved3;
		public Bit Reserved3_
		{
			get
			{
				return _reserved3;
			}
			set
			{
				_reserved3 = value;
			}
		}

		private NoWait _nowait;
		public NoWait NoWait_
		{
			get
			{
				return _nowait;
			}
			set
			{
				_nowait = value;
			}
		}

		/// <summary>
		/// A set of arguments for the declaration. The syntax and semantics of these
		/// arguments depends on the server implementation.
		/// </summary>
		private Table _arguments;
		public Table Arguments_
		{
			get
			{
				return _arguments;
			}
			set
			{
				_arguments = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_exchange = new ExchangeName(reader.ReadShortString());
			_type = new Shortstr(reader.ReadShortString());
			_passive = new Bit(reader.ReadBoolean());
			_durable = new Bit(reader.ReadBoolean());
			_reserved2 = new Bit(reader.ReadBoolean());
			_reserved3 = new Bit(reader.ReadBoolean());
			_nowait = new NoWait(reader.ReadBoolean());
			_arguments = new Table(reader.ReadTable());
		}
	}

	/// <summary>
	/// This method confirms a Declare method and confirms the name of the exchange,
	/// essential for automatically-named exchanges.
	/// </summary>
	public class ExchangeDeclareOk
	{
		public const int ClassId = 40;
		public const int MethodId = 11;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method deletes an exchange. When an exchange is deleted all queue bindings on
	/// the exchange are cancelled.
	/// </summary>
	public class ExchangeDelete
	{
		public const int ClassId = 40;
		public const int MethodId = 20;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		private ExchangeName _exchange;
		public ExchangeName Exchange_
		{
			get
			{
				return _exchange;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_exchange = value;
			}
		}

		/// <summary>
		/// If set, the server will only delete the exchange if it has no queue bindings. If
		/// the exchange has queue bindings the server does not delete it but raises a
		/// channel exception instead.
		/// </summary>
		private Bit _ifunused;
		public Bit IfUnused_
		{
			get
			{
				return _ifunused;
			}
			set
			{
				_ifunused = value;
			}
		}

		private NoWait _nowait;
		public NoWait NoWait_
		{
			get
			{
				return _nowait;
			}
			set
			{
				_nowait = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_exchange = new ExchangeName(reader.ReadShortString());
			_ifunused = new Bit(reader.ReadBoolean());
			_nowait = new NoWait(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method confirms the deletion of an exchange.
	/// </summary>
	public class ExchangeDeleteOk
	{
		public const int ClassId = 40;
		public const int MethodId = 21;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method creates or checks a queue. When creating a new queue the client can
	/// specify various properties that control the durability of the queue and its
	/// contents, and the level of sharing for the queue.
	/// </summary>
	public class QueueDeclare
	{
		public const int ClassId = 50;
		public const int MethodId = 10;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		private QueueName _queue;
		public QueueName Queue_
		{
			get
			{
				return _queue;
			}
			set
			{
				_queue = value;
			}
		}

		/// <summary>
		/// If set, the server will reply with Declare-Ok if the queue already
		/// exists with the same name, and raise an error if not.  The client can
		/// use this to check whether a queue exists without modifying the
		/// server state.  When set, all other method fields except name and no-wait
		/// are ignored.  A declare with both passive and no-wait has no effect.
		/// Arguments are compared for semantic equivalence.
		/// </summary>
		private Bit _passive;
		public Bit Passive_
		{
			get
			{
				return _passive;
			}
			set
			{
				_passive = value;
			}
		}

		/// <summary>
		/// If set when creating a new queue, the queue will be marked as durable. Durable
		/// queues remain active when a server restarts. Non-durable queues (transient
		/// queues) are purged if/when a server restarts. Note that durable queues do not
		/// necessarily hold persistent messages, although it does not make sense to send
		/// persistent messages to a transient queue.
		/// </summary>
		private Bit _durable;
		public Bit Durable_
		{
			get
			{
				return _durable;
			}
			set
			{
				_durable = value;
			}
		}

		/// <summary>
		/// Exclusive queues may only be accessed by the current connection, and are
		/// deleted when that connection closes.  Passive declaration of an exclusive
		/// queue by other connections are not allowed.
		/// </summary>
		private Bit _exclusive;
		public Bit Exclusive_
		{
			get
			{
				return _exclusive;
			}
			set
			{
				_exclusive = value;
			}
		}

		/// <summary>
		/// If set, the queue is deleted when all consumers have finished using it.  The last
		/// consumer can be cancelled either explicitly or because its channel is closed. If
		/// there was no consumer ever on the queue, it won't be deleted.  Applications can
		/// explicitly delete auto-delete queues using the Delete method as normal.
		/// </summary>
		private Bit _autodelete;
		public Bit AutoDelete_
		{
			get
			{
				return _autodelete;
			}
			set
			{
				_autodelete = value;
			}
		}

		private NoWait _nowait;
		public NoWait NoWait_
		{
			get
			{
				return _nowait;
			}
			set
			{
				_nowait = value;
			}
		}

		/// <summary>
		/// A set of arguments for the declaration. The syntax and semantics of these
		/// arguments depends on the server implementation.
		/// </summary>
		private Table _arguments;
		public Table Arguments_
		{
			get
			{
				return _arguments;
			}
			set
			{
				_arguments = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_queue = new QueueName(reader.ReadShortString());
			_passive = new Bit(reader.ReadBoolean());
			_durable = new Bit(reader.ReadBoolean());
			_exclusive = new Bit(reader.ReadBoolean());
			_autodelete = new Bit(reader.ReadBoolean());
			_nowait = new NoWait(reader.ReadBoolean());
			_arguments = new Table(reader.ReadTable());
		}
	}

	/// <summary>
	/// This method confirms a Declare method and confirms the name of the queue, essential
	/// for automatically-named queues.
	/// </summary>
	public class QueueDeclareOk
	{
		public const int ClassId = 50;
		public const int MethodId = 11;
		/// <summary>
		/// Reports the name of the queue. If the server generated a queue name, this field
		/// contains that name.
		/// </summary>
		private QueueName _queue;
		public QueueName Queue_
		{
			get
			{
				return _queue;
			}
			set
			{
				Requires.NotNullAllowStructs(value.Value, nameof(value.Value));
				_queue = value;
			}
		}

		private MessageCount _messagecount;
		public MessageCount MessageCount_
		{
			get
			{
				return _messagecount;
			}
			set
			{
				_messagecount = value;
			}
		}

		/// <summary>
		/// Reports the number of active consumers for the queue. Note that consumers can
		/// suspend activity (Channel.Flow) in which case they do not appear in this count.
		/// </summary>
		private Long _consumercount;
		public Long ConsumerCount_
		{
			get
			{
				return _consumercount;
			}
			set
			{
				_consumercount = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_queue = new QueueName(reader.ReadShortString());
			_messagecount = new MessageCount(reader.ReadLongInteger());
			_consumercount = new Long(reader.ReadLongInteger());
		}
	}

	/// <summary>
	/// This method binds a queue to an exchange. Until a queue is bound it will not
	/// receive any messages. In a classic messaging model, store-and-forward queues
	/// are bound to a direct exchange and subscription queues are bound to a topic
	/// exchange.
	/// </summary>
	public class QueueBind
	{
		public const int ClassId = 50;
		public const int MethodId = 20;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		/// <summary>
		/// Specifies the name of the queue to bind.
		/// </summary>
		private QueueName _queue;
		public QueueName Queue_
		{
			get
			{
				return _queue;
			}
			set
			{
				_queue = value;
			}
		}

		private ExchangeName _exchange;
		public ExchangeName Exchange_
		{
			get
			{
				return _exchange;
			}
			set
			{
				_exchange = value;
			}
		}

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
		private Shortstr _routingkey;
		public Shortstr RoutingKey_
		{
			get
			{
				return _routingkey;
			}
			set
			{
				_routingkey = value;
			}
		}

		private NoWait _nowait;
		public NoWait NoWait_
		{
			get
			{
				return _nowait;
			}
			set
			{
				_nowait = value;
			}
		}

		/// <summary>
		/// A set of arguments for the binding. The syntax and semantics of these arguments
		/// depends on the exchange class.
		/// </summary>
		private Table _arguments;
		public Table Arguments_
		{
			get
			{
				return _arguments;
			}
			set
			{
				_arguments = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_queue = new QueueName(reader.ReadShortString());
			_exchange = new ExchangeName(reader.ReadShortString());
			_routingkey = new Shortstr(reader.ReadShortString());
			_nowait = new NoWait(reader.ReadBoolean());
			_arguments = new Table(reader.ReadTable());
		}
	}

	/// <summary>
	/// This method confirms that the bind was successful.
	/// </summary>
	public class QueueBindOk
	{
		public const int ClassId = 50;
		public const int MethodId = 21;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method unbinds a queue from an exchange.
	/// </summary>
	public class QueueUnbind
	{
		public const int ClassId = 50;
		public const int MethodId = 50;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		/// <summary>
		/// Specifies the name of the queue to unbind.
		/// </summary>
		private QueueName _queue;
		public QueueName Queue_
		{
			get
			{
				return _queue;
			}
			set
			{
				_queue = value;
			}
		}

		/// <summary>
		/// The name of the exchange to unbind from.
		/// </summary>
		private ExchangeName _exchange;
		public ExchangeName Exchange_
		{
			get
			{
				return _exchange;
			}
			set
			{
				_exchange = value;
			}
		}

		/// <summary>
		/// Specifies the routing key of the binding to unbind.
		/// </summary>
		private Shortstr _routingkey;
		public Shortstr RoutingKey_
		{
			get
			{
				return _routingkey;
			}
			set
			{
				_routingkey = value;
			}
		}

		/// <summary>
		/// Specifies the arguments of the binding to unbind.
		/// </summary>
		private Table _arguments;
		public Table Arguments_
		{
			get
			{
				return _arguments;
			}
			set
			{
				_arguments = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_queue = new QueueName(reader.ReadShortString());
			_exchange = new ExchangeName(reader.ReadShortString());
			_routingkey = new Shortstr(reader.ReadShortString());
			_arguments = new Table(reader.ReadTable());
		}
	}

	/// <summary>
	/// This method confirms that the unbind was successful.
	/// </summary>
	public class QueueUnbindOk
	{
		public const int ClassId = 50;
		public const int MethodId = 51;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method removes all messages from a queue which are not awaiting
	/// acknowledgment.
	/// </summary>
	public class QueuePurge
	{
		public const int ClassId = 50;
		public const int MethodId = 30;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		/// <summary>
		/// Specifies the name of the queue to purge.
		/// </summary>
		private QueueName _queue;
		public QueueName Queue_
		{
			get
			{
				return _queue;
			}
			set
			{
				_queue = value;
			}
		}

		private NoWait _nowait;
		public NoWait NoWait_
		{
			get
			{
				return _nowait;
			}
			set
			{
				_nowait = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_queue = new QueueName(reader.ReadShortString());
			_nowait = new NoWait(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method confirms the purge of a queue.
	/// </summary>
	public class QueuePurgeOk
	{
		public const int ClassId = 50;
		public const int MethodId = 31;
		/// <summary>
		/// Reports the number of messages purged.
		/// </summary>
		private MessageCount _messagecount;
		public MessageCount MessageCount_
		{
			get
			{
				return _messagecount;
			}
			set
			{
				_messagecount = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_messagecount = new MessageCount(reader.ReadLongInteger());
		}
	}

	/// <summary>
	/// This method deletes a queue. When a queue is deleted any pending messages are sent
	/// to a dead-letter queue if this is defined in the server configuration, and all
	/// consumers on the queue are cancelled.
	/// </summary>
	public class QueueDelete
	{
		public const int ClassId = 50;
		public const int MethodId = 40;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		/// <summary>
		/// Specifies the name of the queue to delete.
		/// </summary>
		private QueueName _queue;
		public QueueName Queue_
		{
			get
			{
				return _queue;
			}
			set
			{
				_queue = value;
			}
		}

		/// <summary>
		/// If set, the server will only delete the queue if it has no consumers. If the
		/// queue has consumers the server does does not delete it but raises a channel
		/// exception instead.
		/// </summary>
		private Bit _ifunused;
		public Bit IfUnused_
		{
			get
			{
				return _ifunused;
			}
			set
			{
				_ifunused = value;
			}
		}

		/// <summary>
		/// If set, the server will only delete the queue if it has no messages.
		/// </summary>
		private Bit _ifempty;
		public Bit IfEmpty_
		{
			get
			{
				return _ifempty;
			}
			set
			{
				_ifempty = value;
			}
		}

		private NoWait _nowait;
		public NoWait NoWait_
		{
			get
			{
				return _nowait;
			}
			set
			{
				_nowait = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_queue = new QueueName(reader.ReadShortString());
			_ifunused = new Bit(reader.ReadBoolean());
			_ifempty = new Bit(reader.ReadBoolean());
			_nowait = new NoWait(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method confirms the deletion of a queue.
	/// </summary>
	public class QueueDeleteOk
	{
		public const int ClassId = 50;
		public const int MethodId = 41;
		/// <summary>
		/// Reports the number of messages deleted.
		/// </summary>
		private MessageCount _messagecount;
		public MessageCount MessageCount_
		{
			get
			{
				return _messagecount;
			}
			set
			{
				_messagecount = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_messagecount = new MessageCount(reader.ReadLongInteger());
		}
	}

	/// <summary>
	/// This method requests a specific quality of service. The QoS can be specified for the
	/// current channel or for all channels on the connection. The particular properties and
	/// semantics of a qos method always depend on the content class semantics. Though the
	/// qos method could in principle apply to both peers, it is currently meaningful only
	/// for the server.
	/// </summary>
	public class BasicQos
	{
		public const int ClassId = 60;
		public const int MethodId = 10;
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
		private Long _prefetchsize;
		public Long PrefetchSize_
		{
			get
			{
				return _prefetchsize;
			}
			set
			{
				_prefetchsize = value;
			}
		}

		/// <summary>
		/// Specifies a prefetch window in terms of whole messages. This field may be used
		/// in combination with the prefetch-size field; a message will only be sent in
		/// advance if both prefetch windows (and those at the channel and connection level)
		/// allow it. The prefetch-count is ignored if the no-ack option is set.
		/// </summary>
		private Short _prefetchcount;
		public Short PrefetchCount_
		{
			get
			{
				return _prefetchcount;
			}
			set
			{
				_prefetchcount = value;
			}
		}

		/// <summary>
		/// By default the QoS settings apply to the current channel only. If this field is
		/// set, they are applied to the entire connection.
		/// </summary>
		private Bit _global;
		public Bit Global_
		{
			get
			{
				return _global;
			}
			set
			{
				_global = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_prefetchsize = new Long(reader.ReadLongInteger());
			_prefetchcount = new Short(reader.ReadShortInteger());
			_global = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method tells the client that the requested QoS levels could be handled by the
	/// server. The requested QoS applies to all active consumers until a new QoS is
	/// defined.
	/// </summary>
	public class BasicQosOk
	{
		public const int ClassId = 60;
		public const int MethodId = 11;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method asks the server to start a "consumer", which is a transient request for
	/// messages from a specific queue. Consumers last as long as the channel they were
	/// declared on, or until the client cancels them.
	/// </summary>
	public class BasicConsume
	{
		public const int ClassId = 60;
		public const int MethodId = 20;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		/// <summary>
		/// Specifies the name of the queue to consume from.
		/// </summary>
		private QueueName _queue;
		public QueueName Queue_
		{
			get
			{
				return _queue;
			}
			set
			{
				_queue = value;
			}
		}

		/// <summary>
		/// Specifies the identifier for the consumer. The consumer tag is local to a
		/// channel, so two clients can use the same consumer tags. If this field is
		/// empty the server will generate a unique tag.
		/// </summary>
		private ConsumerTag _consumertag;
		public ConsumerTag ConsumerTag_
		{
			get
			{
				return _consumertag;
			}
			set
			{
				_consumertag = value;
			}
		}

		private NoLocal _nolocal;
		public NoLocal NoLocal_
		{
			get
			{
				return _nolocal;
			}
			set
			{
				_nolocal = value;
			}
		}

		private NoAck _noack;
		public NoAck NoAck_
		{
			get
			{
				return _noack;
			}
			set
			{
				_noack = value;
			}
		}

		/// <summary>
		/// Request exclusive consumer access, meaning only this consumer can access the
		/// queue.
		/// </summary>
		private Bit _exclusive;
		public Bit Exclusive_
		{
			get
			{
				return _exclusive;
			}
			set
			{
				_exclusive = value;
			}
		}

		private NoWait _nowait;
		public NoWait NoWait_
		{
			get
			{
				return _nowait;
			}
			set
			{
				_nowait = value;
			}
		}

		/// <summary>
		/// A set of arguments for the consume. The syntax and semantics of these
		/// arguments depends on the server implementation.
		/// </summary>
		private Table _arguments;
		public Table Arguments_
		{
			get
			{
				return _arguments;
			}
			set
			{
				_arguments = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_queue = new QueueName(reader.ReadShortString());
			_consumertag = new ConsumerTag(reader.ReadShortString());
			_nolocal = new NoLocal(reader.ReadBoolean());
			_noack = new NoAck(reader.ReadBoolean());
			_exclusive = new Bit(reader.ReadBoolean());
			_nowait = new NoWait(reader.ReadBoolean());
			_arguments = new Table(reader.ReadTable());
		}
	}

	/// <summary>
	/// The server provides the client with a consumer tag, which is used by the client
	/// for methods called on the consumer at a later stage.
	/// </summary>
	public class BasicConsumeOk
	{
		public const int ClassId = 60;
		public const int MethodId = 21;
		/// <summary>
		/// Holds the consumer tag specified by the client or provided by the server.
		/// </summary>
		private ConsumerTag _consumertag;
		public ConsumerTag ConsumerTag_
		{
			get
			{
				return _consumertag;
			}
			set
			{
				_consumertag = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_consumertag = new ConsumerTag(reader.ReadShortString());
		}
	}

	/// <summary>
	/// This method cancels a consumer. This does not affect already delivered
	/// messages, but it does mean the server will not send any more messages for
	/// that consumer. The client may receive an arbitrary number of messages in
	/// between sending the cancel method and receiving the cancel-ok reply.
	/// </summary>
	public class BasicCancel
	{
		public const int ClassId = 60;
		public const int MethodId = 30;
		private ConsumerTag _consumertag;
		public ConsumerTag ConsumerTag_
		{
			get
			{
				return _consumertag;
			}
			set
			{
				_consumertag = value;
			}
		}

		private NoWait _nowait;
		public NoWait NoWait_
		{
			get
			{
				return _nowait;
			}
			set
			{
				_nowait = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_consumertag = new ConsumerTag(reader.ReadShortString());
			_nowait = new NoWait(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method confirms that the cancellation was completed.
	/// </summary>
	public class BasicCancelOk
	{
		public const int ClassId = 60;
		public const int MethodId = 31;
		private ConsumerTag _consumertag;
		public ConsumerTag ConsumerTag_
		{
			get
			{
				return _consumertag;
			}
			set
			{
				_consumertag = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_consumertag = new ConsumerTag(reader.ReadShortString());
		}
	}

	/// <summary>
	/// This method publishes a message to a specific exchange. The message will be routed
	/// to queues as defined by the exchange configuration and distributed to any active
	/// consumers when the transaction, if any, is committed.
	/// </summary>
	public class BasicPublish
	{
		public const int ClassId = 60;
		public const int MethodId = 40;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		/// <summary>
		/// Specifies the name of the exchange to publish to. The exchange name can be
		/// empty, meaning the default exchange. If the exchange name is specified, and that
		/// exchange does not exist, the server will raise a channel exception.
		/// </summary>
		private ExchangeName _exchange;
		public ExchangeName Exchange_
		{
			get
			{
				return _exchange;
			}
			set
			{
				_exchange = value;
			}
		}

		/// <summary>
		/// Specifies the routing key for the message. The routing key is used for routing
		/// messages depending on the exchange configuration.
		/// </summary>
		private Shortstr _routingkey;
		public Shortstr RoutingKey_
		{
			get
			{
				return _routingkey;
			}
			set
			{
				_routingkey = value;
			}
		}

		/// <summary>
		/// This flag tells the server how to react if the message cannot be routed to a
		/// queue. If this flag is set, the server will return an unroutable message with a
		/// Return method. If this flag is zero, the server silently drops the message.
		/// </summary>
		private Bit _mandatory;
		public Bit Mandatory_
		{
			get
			{
				return _mandatory;
			}
			set
			{
				_mandatory = value;
			}
		}

		/// <summary>
		/// This flag tells the server how to react if the message cannot be routed to a
		/// queue consumer immediately. If this flag is set, the server will return an
		/// undeliverable message with a Return method. If this flag is zero, the server
		/// will queue the message, but with no guarantee that it will ever be consumed.
		/// </summary>
		private Bit _immediate;
		public Bit Immediate_
		{
			get
			{
				return _immediate;
			}
			set
			{
				_immediate = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_exchange = new ExchangeName(reader.ReadShortString());
			_routingkey = new Shortstr(reader.ReadShortString());
			_mandatory = new Bit(reader.ReadBoolean());
			_immediate = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method returns an undeliverable message that was published with the "immediate"
	/// flag set, or an unroutable message published with the "mandatory" flag set. The
	/// reply code and text provide information about the reason that the message was
	/// undeliverable.
	/// </summary>
	public class BasicReturn
	{
		public const int ClassId = 60;
		public const int MethodId = 50;
		private ReplyCode _replycode;
		public ReplyCode ReplyCode_
		{
			get
			{
				return _replycode;
			}
			set
			{
				_replycode = value;
			}
		}

		private ReplyText _replytext;
		public ReplyText ReplyText_
		{
			get
			{
				return _replytext;
			}
			set
			{
				_replytext = value;
			}
		}

		/// <summary>
		/// Specifies the name of the exchange that the message was originally published
		/// to.  May be empty, meaning the default exchange.
		/// </summary>
		private ExchangeName _exchange;
		public ExchangeName Exchange_
		{
			get
			{
				return _exchange;
			}
			set
			{
				_exchange = value;
			}
		}

		/// <summary>
		/// Specifies the routing key name specified when the message was published.
		/// </summary>
		private Shortstr _routingkey;
		public Shortstr RoutingKey_
		{
			get
			{
				return _routingkey;
			}
			set
			{
				_routingkey = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_replycode = new ReplyCode(reader.ReadShortInteger());
			_replytext = new ReplyText(reader.ReadShortString());
			_exchange = new ExchangeName(reader.ReadShortString());
			_routingkey = new Shortstr(reader.ReadShortString());
		}
	}

	/// <summary>
	/// This method delivers a message to the client, via a consumer. In the asynchronous
	/// message delivery model, the client starts a consumer using the Consume method, then
	/// the server responds with Deliver methods as and when messages arrive for that
	/// consumer.
	/// </summary>
	public class BasicDeliver
	{
		public const int ClassId = 60;
		public const int MethodId = 60;
		private ConsumerTag _consumertag;
		public ConsumerTag ConsumerTag_
		{
			get
			{
				return _consumertag;
			}
			set
			{
				_consumertag = value;
			}
		}

		private DeliveryTag _deliverytag;
		public DeliveryTag DeliveryTag_
		{
			get
			{
				return _deliverytag;
			}
			set
			{
				_deliverytag = value;
			}
		}

		private Redelivered _redelivered;
		public Redelivered Redelivered_
		{
			get
			{
				return _redelivered;
			}
			set
			{
				_redelivered = value;
			}
		}

		/// <summary>
		/// Specifies the name of the exchange that the message was originally published to.
		/// May be empty, indicating the default exchange.
		/// </summary>
		private ExchangeName _exchange;
		public ExchangeName Exchange_
		{
			get
			{
				return _exchange;
			}
			set
			{
				_exchange = value;
			}
		}

		/// <summary>
		/// Specifies the routing key name specified when the message was published.
		/// </summary>
		private Shortstr _routingkey;
		public Shortstr RoutingKey_
		{
			get
			{
				return _routingkey;
			}
			set
			{
				_routingkey = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_consumertag = new ConsumerTag(reader.ReadShortString());
			_deliverytag = new DeliveryTag(reader.ReadLongLongInteger());
			_redelivered = new Redelivered(reader.ReadBoolean());
			_exchange = new ExchangeName(reader.ReadShortString());
			_routingkey = new Shortstr(reader.ReadShortString());
		}
	}

	/// <summary>
	/// This method provides a direct access to the messages in a queue using a synchronous
	/// dialogue that is designed for specific types of application where synchronous
	/// functionality is more important than performance.
	/// </summary>
	public class BasicGet
	{
		public const int ClassId = 60;
		public const int MethodId = 70;
		private Short _reserved1;
		public Short Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		/// <summary>
		/// Specifies the name of the queue to get a message from.
		/// </summary>
		private QueueName _queue;
		public QueueName Queue_
		{
			get
			{
				return _queue;
			}
			set
			{
				_queue = value;
			}
		}

		private NoAck _noack;
		public NoAck NoAck_
		{
			get
			{
				return _noack;
			}
			set
			{
				_noack = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Short(reader.ReadShortInteger());
			_queue = new QueueName(reader.ReadShortString());
			_noack = new NoAck(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method delivers a message to the client following a get method. A message
	/// delivered by 'get-ok' must be acknowledged unless the no-ack option was set in the
	/// get method.
	/// </summary>
	public class BasicGetOk
	{
		public const int ClassId = 60;
		public const int MethodId = 71;
		private DeliveryTag _deliverytag;
		public DeliveryTag DeliveryTag_
		{
			get
			{
				return _deliverytag;
			}
			set
			{
				_deliverytag = value;
			}
		}

		private Redelivered _redelivered;
		public Redelivered Redelivered_
		{
			get
			{
				return _redelivered;
			}
			set
			{
				_redelivered = value;
			}
		}

		/// <summary>
		/// Specifies the name of the exchange that the message was originally published to.
		/// If empty, the message was published to the default exchange.
		/// </summary>
		private ExchangeName _exchange;
		public ExchangeName Exchange_
		{
			get
			{
				return _exchange;
			}
			set
			{
				_exchange = value;
			}
		}

		/// <summary>
		/// Specifies the routing key name specified when the message was published.
		/// </summary>
		private Shortstr _routingkey;
		public Shortstr RoutingKey_
		{
			get
			{
				return _routingkey;
			}
			set
			{
				_routingkey = value;
			}
		}

		private MessageCount _messagecount;
		public MessageCount MessageCount_
		{
			get
			{
				return _messagecount;
			}
			set
			{
				_messagecount = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_deliverytag = new DeliveryTag(reader.ReadLongLongInteger());
			_redelivered = new Redelivered(reader.ReadBoolean());
			_exchange = new ExchangeName(reader.ReadShortString());
			_routingkey = new Shortstr(reader.ReadShortString());
			_messagecount = new MessageCount(reader.ReadLongInteger());
		}
	}

	/// <summary>
	/// This method tells the client that the queue has no messages available for the
	/// client.
	/// </summary>
	public class BasicGetEmpty
	{
		public const int ClassId = 60;
		public const int MethodId = 72;
		private Shortstr _reserved1;
		public Shortstr Reserved1_
		{
			get
			{
				return _reserved1;
			}
			set
			{
				_reserved1 = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_reserved1 = new Shortstr(reader.ReadShortString());
		}
	}

	/// <summary>
	/// This method acknowledges one or more messages delivered via the Deliver or Get-Ok
	/// methods. The client can ask to confirm a single message or a set of messages up to
	/// and including a specific message.
	/// </summary>
	public class BasicAck
	{
		public const int ClassId = 60;
		public const int MethodId = 80;
		private DeliveryTag _deliverytag;
		public DeliveryTag DeliveryTag_
		{
			get
			{
				return _deliverytag;
			}
			set
			{
				_deliverytag = value;
			}
		}

		/// <summary>
		/// If set to 1, the delivery tag is treated as "up to and including", so that the
		/// client can acknowledge multiple messages with a single method. If set to zero,
		/// the delivery tag refers to a single message. If the multiple field is 1, and the
		/// delivery tag is zero, tells the server to acknowledge all outstanding messages.
		/// </summary>
		private Bit _multiple;
		public Bit Multiple_
		{
			get
			{
				return _multiple;
			}
			set
			{
				_multiple = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_deliverytag = new DeliveryTag(reader.ReadLongLongInteger());
			_multiple = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method allows a client to reject a message. It can be used to interrupt and
	/// cancel large incoming messages, or return untreatable messages to their original
	/// queue.
	/// </summary>
	public class BasicReject
	{
		public const int ClassId = 60;
		public const int MethodId = 90;
		private DeliveryTag _deliverytag;
		public DeliveryTag DeliveryTag_
		{
			get
			{
				return _deliverytag;
			}
			set
			{
				_deliverytag = value;
			}
		}

		/// <summary>
		/// If requeue is true, the server will attempt to requeue the message.  If requeue
		/// is false or the requeue  attempt fails the messages are discarded or dead-lettered.
		/// </summary>
		private Bit _requeue;
		public Bit Requeue_
		{
			get
			{
				return _requeue;
			}
			set
			{
				_requeue = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_deliverytag = new DeliveryTag(reader.ReadLongLongInteger());
			_requeue = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method asks the server to redeliver all unacknowledged messages on a
	/// specified channel. Zero or more messages may be redelivered.  This method
	/// is deprecated in favour of the synchronous Recover/Recover-Ok.
	/// </summary>
	public class BasicRecoverAsync
	{
		public const int ClassId = 60;
		public const int MethodId = 100;
		/// <summary>
		/// If this field is zero, the message will be redelivered to the original
		/// recipient. If this bit is 1, the server will attempt to requeue the message,
		/// potentially then delivering it to an alternative subscriber.
		/// </summary>
		private Bit _requeue;
		public Bit Requeue_
		{
			get
			{
				return _requeue;
			}
			set
			{
				_requeue = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_requeue = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method asks the server to redeliver all unacknowledged messages on a
	/// specified channel. Zero or more messages may be redelivered.  This method
	/// replaces the asynchronous Recover.
	/// </summary>
	public class BasicRecover
	{
		public const int ClassId = 60;
		public const int MethodId = 110;
		/// <summary>
		/// If this field is zero, the message will be redelivered to the original
		/// recipient. If this bit is 1, the server will attempt to requeue the message,
		/// potentially then delivering it to an alternative subscriber.
		/// </summary>
		private Bit _requeue;
		public Bit Requeue_
		{
			get
			{
				return _requeue;
			}
			set
			{
				_requeue = value;
			}
		}

		public void ReadFrom(AmqpReader reader)
		{
			_requeue = new Bit(reader.ReadBoolean());
		}
	}

	/// <summary>
	/// This method acknowledges a Basic.Recover method.
	/// </summary>
	public class BasicRecoverOk
	{
		public const int ClassId = 60;
		public const int MethodId = 111;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method sets the channel to use standard transactions. The client must use this
	/// method at least once on a channel before using the Commit or Rollback methods.
	/// </summary>
	public class TxSelect
	{
		public const int ClassId = 90;
		public const int MethodId = 10;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method confirms to the client that the channel was successfully set to use
	/// standard transactions.
	/// </summary>
	public class TxSelectOk
	{
		public const int ClassId = 90;
		public const int MethodId = 11;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method commits all message publications and acknowledgments performed in
	/// the current transaction.  A new transaction starts immediately after a commit.
	/// </summary>
	public class TxCommit
	{
		public const int ClassId = 90;
		public const int MethodId = 20;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method confirms to the client that the commit succeeded. Note that if a commit
	/// fails, the server raises a channel exception.
	/// </summary>
	public class TxCommitOk
	{
		public const int ClassId = 90;
		public const int MethodId = 21;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method abandons all message publications and acknowledgments performed in
	/// the current transaction. A new transaction starts immediately after a rollback.
	/// Note that unacked messages will not be automatically redelivered by rollback;
	/// if that is required an explicit recover call should be issued.
	/// </summary>
	public class TxRollback
	{
		public const int ClassId = 90;
		public const int MethodId = 30;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}

	/// <summary>
	/// This method confirms to the client that the rollback succeeded. Note that if an
	/// rollback fails, the server raises a channel exception.
	/// </summary>
	public class TxRollbackOk
	{
		public const int ClassId = 90;
		public const int MethodId = 31;

		public void ReadFrom(AmqpReader reader)
		{

		}
	}
}