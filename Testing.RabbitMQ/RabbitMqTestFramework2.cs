using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.RabbitMQ.MessageClient;
using Test.It.With.RabbitMQ.NetworkClient;
using Test.It.With.RabbitMQ.Protocol;
using Constants = RabbitMQ.Client.Framing.Constants;
using IProtocol = RabbitMQ.Client.IProtocol;

namespace Test.It.With.RabbitMQ
{
    public class MethodFrame<TMethod> : TestFrame where TMethod : Amqp.Protocol.IMethod
    {
        public MethodFrame(short channel, TMethod method)
        {
            Channel = channel;
            Method = method;
        }

        public override short Channel { get; }
        public TMethod Method { get; }
    }

    public abstract class TestFrame
    {
        public abstract short Channel { get; }
    }

    public class MethodFrame : TestFrame
    {
        public MethodFrame(short channel, Amqp.Protocol.IMethod method)
        {
            Channel = channel;
            Method = method;
        }

        public override short Channel { get; }
        public Amqp.Protocol.IMethod Method { get; }
    }

    public class RabbitMqTestFramework2 : IDisposable
    {
        private readonly INetworkClient _serverNetworkClient;
        private FrameClient _frameClient;
        private MethodFrameClient _methodFrameClient;

        public RabbitMqTestFramework2()
        {
            var networkClientFactory = new InternalRoutedNetworkClientFactory();
            _serverNetworkClient = networkClientFactory.ServerNetworkClient;
            ConnectionFactory = new TestConnectionFactory();

            _frameClient = new FrameClient(_serverNetworkClient);
            _methodFrameClient = new MethodFrameClient(_frameClient, new AmqProtocol());
        }

        public IConnectionFactory ConnectionFactory { get; }

        public void Send<TMessage>(MethodFrame<TMessage> frame) where TMessage : Amqp.Protocol.IMethod
        {
            _frameClient.Send(new Frame(Constants.FrameMethod, frame.Channel, frame.Method));
        }

        public void On<TMessage>(Action<MethodFrame<TMessage>> messageHandler) 
            where TMessage : Amqp.Protocol.IMethod
        {
            var client = new MethodFrameClient<TMessage>(_methodFrameClient);
            client.Received += (sender, frame) =>
            {
                messageHandler(frame);
            };
        }

        public void On<TMessage, TRespondMethod>(Func<MethodFrame<TMessage>, TRespondMethod> messageHandler) 
            where TMessage : Amqp.Protocol.IMethod, IRespond<TRespondMethod>
            where TRespondMethod : Amqp.Protocol.IMethod
        {
            var client = new MethodFrameClient<TMessage>(_methodFrameClient);
            client.Received += (sender, frame) =>
            {
                var response = messageHandler(frame);
                client.Send(new Frame(Constants.FrameMethod, frame.Channel, response));
            };
        }


        public void Dispose()
        {
            _serverNetworkClient.Dispose();
        }
    }
    
    internal class TestAuthMechanism : AuthMechanism
    {
        private readonly Func<byte[], IConnectionFactory, byte[]> _callengeHandler;

        public TestAuthMechanism(Func<byte[], IConnectionFactory, byte[]> callengeHandler)
        {
            _callengeHandler = callengeHandler;
        }

        public byte[] handleChallenge(byte[] challenge, IConnectionFactory factory)
        {
            return _callengeHandler(challenge, factory);
        }
    }

    internal class TestAuthMechanismFactory : AuthMechanismFactory
    {
        private readonly Func<byte[], IConnectionFactory, byte[]> _callengeHandler;

        public TestAuthMechanismFactory(Func<byte[], IConnectionFactory, byte[]> callengeHandler)
        {
            _callengeHandler = callengeHandler;
        }

        public AuthMechanism GetInstance()
        {
            return new TestAuthMechanism(_callengeHandler);
        }

        public string Name { get; } = "Test";
    }

    internal class TestConnectionFactory : IConnectionFactory
    {
        public Uri Uri { get; set; }

        public AuthMechanismFactory AuthMechanismFactory(IList<string> mechanismNames)
        {
            // Todo: add test delgate to customize handler
            return new TestAuthMechanismFactory((bytes, factory) => new byte[0]);
        }

        public IConnection CreateConnection()
        {
            return new TestConnection();
        }

        public IConnection CreateConnection(string clientProvidedName)
        {
            return new TestConnection(clientProvidedName);
        }

        public IConnection CreateConnection(IList<string> hostnames)
        {
            return new TestConnection(hostnames);
        }

        public IConnection CreateConnection(IList<string> hostnames, string clientProvidedName)
        {
            return new TestConnection(hostnames, clientProvidedName);
        }

        public IConnection CreateConnection(IList<AmqpTcpEndpoint> endpoints)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> ClientProperties { get; set; }
        public string Password { get; set; }
        public ushort RequestedChannelMax { get; set; }
        public uint RequestedFrameMax { get; set; }
        public ushort RequestedHeartbeat { get; set; }
        public bool UseBackgroundThreadsForIO { get; set; }
        public string UserName { get; set; }
        public string VirtualHost { get; set; }
        public TaskScheduler TaskScheduler { get; set; }
        public TimeSpan HandshakeContinuationTimeout { get; set; }
        public TimeSpan ContinuationTimeout { get; set; }
    }

    internal class TestConnection : IConnection
    {
        public IList<string> Hostnames { get; } = new List<string>();
        public string ClientProviderName { get; } = "";

        private List<IModel> _models = new List<IModel>();

        public TestConnection()
        {

        }

        public TestConnection(string clientProviderName) : this(new List<string>(), clientProviderName)
        {
        }

        public TestConnection(IList<string> hostnames) : this(hostnames, string.Empty)
        {
        }

        public TestConnection(IList<string> hostnames, string clientProviderName)
        {
            Hostnames = hostnames;
            ClientProviderName = clientProviderName;
        }

        public int LocalPort { get; } = 0;
        public int RemotePort { get; } = 0;
        public void Dispose()
        {
            _models.Clear();
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public void Abort(ushort reasonCode, string reasonText)
        {
            throw new NotImplementedException();
        }

        public void Abort(int timeout)
        {
            throw new NotImplementedException();
        }

        public void Abort(ushort reasonCode, string reasonText, int timeout)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Close(ushort reasonCode, string reasonText)
        {
            throw new NotImplementedException();
        }

        public void Close(int timeout)
        {
            throw new NotImplementedException();
        }

        public void Close(ushort reasonCode, string reasonText, int timeout)
        {
            throw new NotImplementedException();
        }

        public IModel CreateModel()
        {
            var model = new TestModel();
            _models.Add(model);
            model.OnDispose += (sender, args) => _models.Remove(model);
            return model;
        }

        public void HandleConnectionBlocked(string reason)
        {
            throw new NotImplementedException();
        }

        public void HandleConnectionUnblocked()
        {
            throw new NotImplementedException();
        }

        public bool AutoClose { get; set; }
        public ushort ChannelMax { get; }
        public IDictionary<string, object> ClientProperties { get; }
        public ShutdownEventArgs CloseReason { get; }
        public AmqpTcpEndpoint Endpoint { get; }
        public uint FrameMax { get; }
        public ushort Heartbeat { get; }
        public bool IsOpen { get; }
        public AmqpTcpEndpoint[] KnownHosts { get; }
        public IProtocol Protocol { get; }
        public IDictionary<string, object> ServerProperties { get; }
        public IList<ShutdownReportEntry> ShutdownReport { get; }
        public string ClientProvidedName { get; }
        public ConsumerWorkService ConsumerWorkService { get; }
        public event EventHandler<CallbackExceptionEventArgs> CallbackException;
        public event EventHandler<EventArgs> RecoverySucceeded;
        public event EventHandler<ConnectionRecoveryErrorEventArgs> ConnectionRecoveryError;
        public event EventHandler<ConnectionBlockedEventArgs> ConnectionBlocked;
        public event EventHandler<ShutdownEventArgs> ConnectionShutdown;
        public event EventHandler<EventArgs> ConnectionUnblocked;
    }

    internal class TestModel : IModel
    {
        public bool SimulateNoConnection { get; set; }
        public bool SimulateBrokerUnreachable { get; set; }
        public int NoOfPublishes { get; private set; }


        private string GetPublishedSubscriptionKey(string exchangeId, string routingKey)
        {
            return $"{exchangeId}-{routingKey}";
        }
        public delegate void PublishedDelegate(PublishedProperties properies, byte[] payload);

        public TestModel OnPublished(string exchange, string routingKey, PublishedDelegate publishedDelegate)
        {
            _onPublishedSubscriptions.Add(new KeyValuePair<string, PublishedDelegate>(GetPublishedSubscriptionKey(exchange, routingKey), publishedDelegate));
            return this;
        }

        private List<KeyValuePair<string, PublishedDelegate>> _onPublishedSubscriptions = new List<KeyValuePair<string, PublishedDelegate>>();

        public event EventHandler OnDispose;
        public void Dispose()
        {
            OnDispose?.Invoke(this, null);
        }

        public void Abort()
        {
            OnAbort?.Invoke(this, new AbortEventArgs());
        }

        public event EventHandler<AbortEventArgs> OnAbort;
        public void Abort(ushort replyCode, string replyText)
        {
            OnAbort?.Invoke(this, new AbortEventArgs
            {
                ReplyCode = replyCode,
                ReplyText = replyText
            });
        }

        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            BasicAcks?.Invoke(this, new BasicAckEventArgs { DeliveryTag = deliveryTag, Multiple = multiple });
        }

        public event EventHandler<BasicCancelEventArgs> OnBasicCancel;
        public void BasicCancel(string consumerTag)
        {
            OnBasicCancel?.Invoke(this, new BasicCancelEventArgs(consumerTag));
        }

        public string BasicConsume(string queue, bool noAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object> arguments,
            IBasicConsumer consumer)
        {
            throw new NotImplementedException();
        }

        public BasicGetResult BasicGet(string queue, bool noAck)
        {
            throw new NotImplementedException();
        }

        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            throw new NotImplementedException();
        }


        private void ThrowIfSimulatingNoConnection()
        {
            if (SimulateNoConnection)
            {
                throw new IOException("Simulating connection problems.");
            }
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            ThrowIfSimulatingNoConnection();

            NoOfPublishes++;

            var properties = new PublishedProperties
            {
                Type = basicProperties.Type,
                CorrelationId = basicProperties.CorrelationId,
                AppId = basicProperties.AppId,
                ClusterId = basicProperties.ClusterId,
                ContentEncoding = basicProperties.ContentEncoding,
                ContentType = basicProperties.ContentType,
                DeliveryMode = basicProperties.DeliveryMode,
                Expiration = basicProperties.Expiration,
                Headers = basicProperties.Headers,
                Mandatory = mandatory,
                MessageId = basicProperties.MessageId,
                Persistent = basicProperties.Persistent,
                Priority = basicProperties.Priority,
                ReplyTo = basicProperties.ReplyTo,
                ReplyToAddress = Map(basicProperties.ReplyToAddress),
                Timestamp = ConvertUnixTimeStampToUtcDateTime(basicProperties.Timestamp.UnixTime),
                UserId = basicProperties.UserId
            };

            var subscriptions = _onPublishedSubscriptions
                .Where(pair => pair.Key == GetPublishedSubscriptionKey(exchange, routingKey))
                .Select(pair => pair.Value);

            foreach (var subscription in subscriptions)
            {
                subscription(properties, body);
            }
        }

        private static Address Map(PublicationAddress address)
        {
            if (address == null)
            {
                return null;
            }

            return new Address
            {
                ExchangeName = address.ExchangeName,
                ExchangeType = address.ExchangeType,
                RoutingKey = address.RoutingKey
            };
        }

        private static DateTime ConvertUnixTimeStampToUtcDateTime(double unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }

        public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            throw new NotImplementedException();
        }

        public void BasicRecover(bool requeue)
        {
            throw new NotImplementedException();
        }

        public void BasicRecoverAsync(bool requeue)
        {
            throw new NotImplementedException();
        }

        public void BasicReject(ulong deliveryTag, bool requeue)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Close(ushort replyCode, string replyText)
        {
            throw new NotImplementedException();
        }

        public void ConfirmSelect()
        {
            throw new NotImplementedException();
        }

        public IBasicProperties CreateBasicProperties()
        {
            throw new NotImplementedException();
        }

        public void ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeBindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclareNoWait(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclarePassive(string exchange)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDelete(string exchange, bool ifUnused)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeleteNoWait(string exchange, bool ifUnused)
        {
            throw new NotImplementedException();
        }

        public void ExchangeUnbind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeUnbindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void QueueBindNoWait(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void QueueDeclareNoWait(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public QueueDeclareOk QueueDeclarePassive(string queue)
        {
            throw new NotImplementedException();
        }

        public uint MessageCount(string queue)
        {
            throw new NotImplementedException();
        }

        public uint ConsumerCount(string queue)
        {
            throw new NotImplementedException();
        }

        public uint QueueDelete(string queue, bool ifUnused, bool ifEmpty)
        {
            throw new NotImplementedException();
        }

        public void QueueDeleteNoWait(string queue, bool ifUnused, bool ifEmpty)
        {
            throw new NotImplementedException();
        }

        public uint QueuePurge(string queue)
        {
            throw new NotImplementedException();
        }

        public void QueueUnbind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void TxCommit()
        {
            throw new NotImplementedException();
        }

        public void TxRollback()
        {
            throw new NotImplementedException();
        }

        public void TxSelect()
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms()
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms(TimeSpan timeout, out bool timedOut)
        {
            throw new NotImplementedException();
        }

        public void WaitForConfirmsOrDie()
        {
            throw new NotImplementedException();
        }

        public void WaitForConfirmsOrDie(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public int ChannelNumber { get; private set; }
        public ShutdownEventArgs CloseReason { get; set; }
        public IBasicConsumer DefaultConsumer { get; set; }
        public bool IsClosed { get; set; }
        public bool IsOpen { get; set; }
        public ulong NextPublishSeqNo { get; set; }
        public TimeSpan ContinuationTimeout { get; set; }
        public event EventHandler<BasicAckEventArgs> BasicAcks;
        public event EventHandler<BasicNackEventArgs> BasicNacks;
        public event EventHandler<EventArgs> BasicRecoverOk;
        public event EventHandler<BasicReturnEventArgs> BasicReturn;
        public event EventHandler<CallbackExceptionEventArgs> CallbackException;
        public event EventHandler<FlowControlEventArgs> FlowControl;
        public event EventHandler<ShutdownEventArgs> ModelShutdown;
    }

    internal class PublishedProperties
    {
        public string AppId { get; set; }

        /// <summary>
        /// Intra-cluster routing identifier (cluster id is deprecated in AMQP 0-9-1).
        /// </summary>
        public string ClusterId { get; set; }

        /// <summary>MIME content encoding.</summary>
        public string ContentEncoding { get; set; }

        /// <summary>MIME content type.</summary>
        public string ContentType { get; set; }

        /// <summary>Application correlation identifier.</summary>
        public string CorrelationId { get; set; }

        /// <summary>Non-persistent (1) or persistent (2).</summary>
        public byte DeliveryMode { get; set; }

        /// <summary>Message expiration specification.</summary>
        public string Expiration { get; set; }

        /// <summary>
        /// Message header field table. Is of type <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

        /// <summary>Application message Id.</summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Sets <see cref="P:RabbitMQ.Client.IBasicProperties.DeliveryMode" /> to either persistent (2) or non-persistent (1).
        /// </summary>
        public bool Persistent { get; set; }

        /// <summary>Message priority, 0 to 9.</summary>
        public byte Priority { get; set; }

        /// <summary>Destination to reply to.</summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Convenience property; parses <see cref="P:RabbitMQ.Client.IBasicProperties.ReplyTo" /> property using <see cref="M:RabbitMQ.Client.PublicationAddress.Parse(System.String)" />,
        /// and serializes it using <see cref="M:RabbitMQ.Client.PublicationAddress.ToString" />.
        /// Returns null if <see cref="P:RabbitMQ.Client.IBasicProperties.ReplyTo" /> property cannot be parsed by <see cref="M:RabbitMQ.Client.PublicationAddress.Parse(System.String)" />.
        /// </summary>
        public Address ReplyToAddress { get; set; }

        /// <summary>Message timestamp.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Message type name.</summary>
        public string Type { get; set; }

        /// <summary>User Id.</summary>
        public string UserId { get; set; }

        public bool Mandatory { get; set; }
    }

    internal class Address
    {
        public string ExchangeName { get; set; }

        public string ExchangeType { get; set; }

        public string RoutingKey { get; set; }
    }

    public class BasicCancelEventArgs
    {
        public BasicCancelEventArgs(string consumerTag)
        {
            ConsumerTag = consumerTag;
        }

        public string ConsumerTag { get; }
    }

    public class AbortEventArgs
    {
        public string ReplyText { get; set; }
        public ushort ReplyCode { get; set; }
    }
}