using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using log4net;
using NServiceBus;
using NServiceBus.MessageInterfaces;
using NServiceBus.Messages;
using NServiceBus.ObjectBuilder;
using NServiceBus.Saga;
using NServiceBus.Unicast;
using NServiceBus.Unicast.Subscriptions;
using NServiceBus.Unicast.Transport;
using NServiceBus.Utils;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.Ticker;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
	/// <summary>
	/// A unicast implementation of <see cref="IBus"/> for NServiceBus.
	/// </summary>
	public class TpUnicastBus : IBusExtended, IStartableBus
	{
		/// <summary>
		/// to reduce log file size, we need to skip some messages that are sent frequently.
		/// </summary>
		private readonly IEnumerable<Type> _messageTypesNotToLog = new List<Type> { typeof(CheckIntervalElapsedMessage) };

		/// <summary>
		/// Header entry key for the given message type that is being subscribed to, when message intent is subscribe or unsubscribe.
		/// </summary>
		public const string SubscriptionMessageType = "SubscriptionMessageType";

		/// <summary>
		/// Header entry key indicating the types of messages contained.
		/// </summary>
		public const string EnclosedMessageTypes = "EnclosedMessageTypes";

		#region config properties

		private bool _autoSubscribe = true;

		public string Proxy { get; set; }

		/// <summary>
		/// When set, when starting up, the bus performs 
		/// a subscribe operation for message types for which it has
		/// handlers and that are owned by a different endpoint.
		/// Default is true.
		/// </summary>
		public bool AutoSubscribe
		{
			get { return _autoSubscribe; }
			set { _autoSubscribe = value; }
		}

		private bool disableMessageHandling;

		/// <summary>
		/// Should be used by programmer, not administrator.
		/// Disables the handling of incoming messages.
		/// </summary>
		public virtual bool DisableMessageHandling
		{
			set { disableMessageHandling = value; }
		}

		/// <summary>
		/// A reference to the transport.
		/// </summary>
		protected IMsmqTransport transport;

		/// <summary>
		/// Should be used by programmer, not administrator.
		/// Sets an <see cref="ITransport"/> implementation to use as the
		/// message transport for the bus.
		/// </summary>
		public virtual IMsmqTransport Transport
		{
			set
			{
				transport = value;

				transport.StartedMessageProcessing += TransportStartedMessageProcessing;
				transport.TransportMessageReceived += TransportMessageReceived;
				transport.FinishedMessageProcessing += TransportFinishedMessageProcessing;
				transport.FailedMessageProcessing += TransportFailedMessageProcessing;
			}
		}

		/// <summary>
		/// A delegate for a method that will handle the <see cref="MessageReceived"/>
		/// event.
		/// </summary>
		/// <param name="message">The message received.</param>
		public delegate void MessageReceivedDelegate(TransportMessage message);

		/// <summary>
		/// Event raised when a message is received.
		/// </summary>
		public event MessageReceivedDelegate MessageReceived;

		/// <summary>
		/// Should be used by programmer, not administrator.
		/// Sets an <see cref="ISubscriptionStorage"/> implementation to
		/// be used for subscription storage for the bus.
		/// </summary>
		public virtual ISubscriptionStorage SubscriptionStorage
		{
			set { subscriptionStorage = value; }
		}

		/// <summary>
		/// Should be used by programmer, not administrator.
		/// Sets <see cref="IBuilder"/> implementation that will be used to 
		/// dynamically instantiate and execute message handlers.
		/// </summary>
		public IBuilder Builder { get; set; }

		private IMessageMapper messageMapper;

		/// <summary>
		/// Gets/sets the message mapper.
		/// </summary>
		public virtual IMessageMapper MessageMapper
		{
			get { return messageMapper; }
			set
			{
				messageMapper = value;

				ExtensionMethods.MessageCreator = value;
				ExtensionMethods.Bus = this;
			}
		}

		/// <summary>
		/// Should be used by programmer, not administrator.
		/// Sets whether or not the return address of a received message 
		/// should be propogated when the message is forwarded. This field is
		/// used primarily for the Distributor.
		/// </summary>
		public bool PropogateReturnAddressOnSend { get; set; }

		/// <summary>
		/// Should be used by programmer, not administrator.
		/// Sets whether or not the bus should impersonate the sender
		/// of a message it has received when re-sending the message.
		/// What occurs is that the thread sets its current principal
		/// to the value found in the <see cref="TransportMessage.WindowsIdentityName" />
		/// when that thread handles a message.
		/// </summary>
		public virtual bool ImpersonateSender { get; set; }

		/// <summary>
		/// Should be used by administrator, not programmer.
		/// Sets the address to which the messages received on this bus
		/// will be sent when the method HandleCurrentMessageLater is called.
		/// </summary>
		public string DistributorDataAddress { get; set; }

		/// <summary>
		/// Should be used by administrator, not programmer.
		/// Sets the address of the distributor control queue.
		/// </summary>
		/// <remarks>
		/// Notifies the given distributor
		/// when a thread is now available to handle a new message.
		/// </remarks>
		public string DistributorControlAddress { get; set; }

		/// <summary>
		/// Should be used by administrator, not programmer.
		/// Sets the address to which all messages received on this bus will be 
		/// forwarded to (not including subscription messages). 
		/// This is primarily useful for smart client scenarios 
		/// where both client and server software are installed on the mobile
		/// device. The server software will have this field set to the address
		/// of the real server.
		/// </summary>
		public string ForwardReceivedMessagesTo { get; set; }

		/// <summary>
		/// Should be used by administrator, not programmer.
		/// Sets the message types associated with the bus.
		/// </summary>
		/// <remarks>
		/// This property accepts a dictionary where the key can be the name of a type implementing
		/// <see cref="IMessage"/> or the name of an assembly that contains message types.  The value 
		/// of each entry is the address of the owner of the message type defined in the key.
		/// If an assembly is specified then all the the types in the assembly implementing <see cref="IMessage"/> 
		/// will be registered against the address defined in the value of the entry.
		/// </remarks>
		public IDictionary MessageOwners
		{
			get { return messageOwners; }
			set
			{
				messageOwners = value;
				ConfigureMessageOwners(value);
			}
		}

		private IDictionary messageOwners;

		/// <summary>
		/// Sets the list of assemblies which contain a message handlers
		/// for the bus.
		/// </summary>
		public virtual IList MessageHandlerAssemblies
		{
			set
			{
				var types = new List<Type>();
				foreach (Assembly a in value)
				{
					types.AddRange(a.GetTypes());
				}

				MessageHandlerTypes = types;
			}
		}

		/// <summary>
		/// Sets the types that will be scanned for message handlers.
		/// Those found will be invoked in the same order as given.
		/// </summary>
		public IEnumerable<Type> MessageHandlerTypes
		{
			get { return messageHandlerTypes; }
			set
			{
				messageHandlerTypes = value;

				foreach (Type t in value)
				{
					IfTypeIsMessageHandlerThenLoad(t);
				}
			}
		}

		private IEnumerable<Type> messageHandlerTypes;

		public IEnumerable<Type> SagaMessageHandlerTypes
		{
			get { return sagaMessageHandlerTypes; }
			set
			{
				sagaMessageHandlerTypes = value;

				foreach (Type t in value)
				{
					IfTypeIsSagaMessageHandlerThenLoad(t);
				}
			}
		}

		private IEnumerable<Type> sagaMessageHandlerTypes;


		/// <summary>
		/// Object that will be used to authorize subscription requests.
		/// </summary>
		public IAuthorizeSubscriptions SubscriptionAuthorizer { get; set; }

		#endregion

		#region IUnicastBus Members

		/// <summary>
		/// Stops sending ready messages to the distributor, if one is configured.
		/// </summary>
		public void StopSendingReadyMessages()
		{
			_canSendReadyMessages = false;
		}

		/// <summary>
		/// Continues sending ready messages to the distributor, if one is configured.
		/// </summary>
		public void ContinueSendingReadyMessages()
		{
			_canSendReadyMessages = true;
		}

		/// <summary>
		/// Skips sending a ready message to the distributor once.
		/// </summary>
		public void SkipSendingReadyMessageOnce()
		{
			_skipSendingReadyMessageOnce = true;
		}

		/// <summary>
		/// Event raised when no subscribers found for the published message.
		/// </summary>
		public event EventHandler<MessageEventArgs> NoSubscribersForMessage;

		/// <summary>
		/// Event raised when client subscribed to a message type.
		/// </summary>
		public event EventHandler<SubscriptionEventArgs> ClientSubscribed;

		#endregion

		#region IBus Members

		/// <summary>
		/// Creates an instance of the specified type.
		/// Used primarily for instantiating interface-based messages.
		/// </summary>
		/// <typeparam name="T">The type to instantiate.</typeparam>
		/// <returns>An instance of the specified type.</returns>
		public T CreateInstance<T>() where T : IMessage
		{
			return messageMapper.CreateInstance<T>();
		}

		/// <summary>
		/// Creates an instance of the specified type.
		/// Used primarily for instantiating interface-based messages.
		/// </summary>
		/// <typeparam name="T">The type to instantiate.</typeparam>
		/// <param name="action">An action to perform on the result</param>
		/// <returns></returns>
		public T CreateInstance<T>(Action<T> action) where T : IMessage
		{
			return messageMapper.CreateInstance(action);
		}

		/// <summary>
		/// Creates an instance of the specified type.
		/// Used primarily for instantiating interface-based messages.
		/// </summary>
		/// <param name="messageType">The type to instantiate.</param>
		/// <returns>An instance of the specified type.</returns>
		public object CreateInstance(Type messageType)
		{
			return messageMapper.CreateInstance(messageType);
		}

		/// <summary>
		/// Creates an instance of the requested message type (T), 
		/// performing the given action on the created message,
		/// and then publishing it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="messageConstructor"></param>
		public void Publish<T>(Action<T> messageConstructor) where T : IMessage
		{
			Publish(CreateInstance(messageConstructor));
		}

		/// <summary>
		/// Publishes the messages to all subscribers of the first message's type.
		/// </summary>
		/// <param name="messages"></param>
		public virtual void Publish<T>(params T[] messages) where T : IMessage
		{
			var subscribers = GetSubscribers(messages);

			PublishInternal(subscribers, messages);
		}

		private IList<string> GetSubscribers<T>(T[] messages) where T : IMessage
		{
			if (subscriptionStorage == null)
				throw new InvalidOperationException(
					"Cannot publish on this endpoint - no subscription storage has been configured. Add either 'MsmqSubscriptionStorage()' or 'DbSubscriptionStorage()' after 'NServiceBus.Configure.With()'.");

			return subscriptionStorage.GetSubscribersForMessage(GetFullTypes(messages as IMessage[]));
		}

		public virtual void PublishToUi<T>(params T[] messages) where T : IMessage
		{
			var subscribers = GetSubscribers(messages);

			var uiQueues = new List<string>();
			foreach (var subscriber in subscribers)
			{
				uiQueues.Add(GetUiQueueName(subscriber));
			}
			PublishInternal(uiQueues, messages);
		}

		private void PublishInternal<T>(IList<string> subscribers, params T[] messages) where T : IMessage
		{
			if (subscribers.Count == 0)
			{
				NoSubscribersForMessage?.Invoke(this, new MessageEventArgs(messages[0]));
			}

			SendMessage(subscribers, null, MessageIntentEnum.Publish, messages as IMessage[]);
		}

		/// <summary>
		/// Subscribes to the given type - T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Subscribe<T>() where T : IMessage
		{
			Subscribe(typeof(T));
		}

		/// <summary>
		/// Subcribes to recieve published messages of the specified type.
		/// </summary>
		/// <param name="messageType">The type of message to subscribe to.</param>
		public virtual void Subscribe(Type messageType)
		{
			Subscribe(messageType, null);
		}

		/// <summary>
		/// Subscribes to the given type T, registering a condition that all received
		/// messages of that type should comply with, otherwise discarding them.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="condition"></param>
		public void Subscribe<T>(Predicate<T> condition) where T : IMessage
		{
			var p = new Predicate<IMessage>(m =>
			{
				if (m is T)
					return condition((T) m);

				return true;
			}
				);

			Subscribe(typeof(T), p);
		}

		/// <summary>
		/// Subscribes to receive published messages of the specified type if
		/// they meet the provided condition.
		/// </summary>
		/// <param name="messageType">The type of message to subscribe to.</param>
		/// <param name="condition">The condition under which to receive the message.</param>
		public virtual void Subscribe(Type messageType, Predicate<IMessage> condition)
		{
			AssertBusIsStarted();

			subscriptionsManager.AddConditionForSubscriptionToMessageType(messageType, condition);

			var destination = GetDestinationForMessageType(messageType);

			if (destination == null)
				throw new InvalidOperationException(
					$"No destination could be found for message type {messageType}. " +
						"Check the <MessageEndpointMapping> section of the configuration of this endpoint for an entry either for this specific message type or for its assembly.");

			Log.Info("Subscribing to " + messageType.AssemblyQualifiedName + " at publisher queue " + destination);

			((IBus) this).OutgoingHeaders[SubscriptionMessageType] = messageType.AssemblyQualifiedName;
			SendMessage(destination, null, MessageIntentEnum.Subscribe, new CompletionMessage());
			((IBus) this).OutgoingHeaders.Remove(SubscriptionMessageType);
		}

		/// <summary>
		/// Unsubscribes from the given type of message - T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Unsubscribe<T>() where T : IMessage
		{
			Unsubscribe(typeof(T));
		}

		/// <summary>
		/// Unsubscribes from receiving published messages of the specified type.
		/// </summary>
		/// <param name="messageType"></param>
		public virtual void Unsubscribe(Type messageType)
		{
			var destination = GetDestinationForMessageType(messageType);

			if (destination == null)
				throw new InvalidOperationException(
					$"No destination could be found for message type {messageType}. " +
						"Check the <MessageEndpointMapping> section of the configuration of this endpoint for an entry either for this specific message type or for its assembly.");

			Log.Info("Unsubscribing from " + messageType.AssemblyQualifiedName + " at publisher queue " + destination);

			((IBus) this).OutgoingHeaders[SubscriptionMessageType] = messageType.AssemblyQualifiedName;
			SendMessage(destination, null, MessageIntentEnum.Unsubscribe, new CompletionMessage());
			((IBus) this).OutgoingHeaders.Remove(SubscriptionMessageType);
		}

		/// <summary>
		/// Gets name for UI queue
		/// </summary>
		/// <param name="queueName"></param>
		/// <returns></returns>
		public static string GetUiQueueName(string queueName)
		{
			if (string.IsNullOrEmpty(queueName)) return null;

			var q = MsmqUtilities.GetQueueNameFromLogicalName(queueName);
			return queueName.Replace(q, $"{q}UI");
		}

		public static bool IsUiQueue(string queueName)
		{
			var q = MsmqUtilities.GetQueueNameFromLogicalName(queueName);
			return q.EndsWith("UI");
		}

		void IBus.Reply(params IMessage[] messages)
		{
			ReplyInternal(_messageBeingHandled.ReturnAddress, messages);
		}

		void IBus.ReplyToUi(params IMessage[] messages)
		{
			ReplyInternal(GetUiQueueName(_messageBeingHandled.ReturnAddress), messages);
		}

		void ReplyInternal(string address, params IMessage[] messages)
		{
			var from = ExtensionMethods.CurrentMessageBeingHandled.GetHttpFromHeader();
			if (from != null)
				messages[0].SetHttpToHeader(from);

			messages[0].CopyHeaderFromRequest("ReturnAddress");

			SendMessage(address, _messageBeingHandled.IdForCorrelation, MessageIntentEnum.Send, messages);
		}

		void IBus.Reply<T>(Action<T> messageConstructor)
		{
			((IBus) this).Reply(CreateInstance(messageConstructor));
		}

		void IBus.Return(int errorCode)
		{
			((IBus) this).Reply(new CompletionMessage { ErrorCode = errorCode });
		}

		void IBus.HandleCurrentMessageLater()
		{
			if (_handleCurrentMessageLaterWasCalled)
				return;

			if (DistributorDataAddress != null)
				transport.Send(_messageBeingHandled, DistributorDataAddress);
			else
				transport.ReceiveMessageLater(_messageBeingHandled);

			_handleCurrentMessageLaterWasCalled = true;
		}

		void IBus.ForwardCurrentMessageTo(string destination)
		{
			transport.Send(_messageBeingHandled, destination);
		}

		/// <summary>
		/// ThreadStatic variable indicating if the current message was already
		/// marked to be handled later so we don't do this more than once.
		/// </summary>
		[ThreadStatic] private static bool _handleCurrentMessageLaterWasCalled;

		void IBus.SendLocal<T>(Action<T> messageConstructor)
		{
			SendLocal(CreateInstance(messageConstructor));
		}

		public void SendLocalUi(params IMessage[] messages)
		{
			var m = GetTransportMessageFor(messages);

			var uiQueueAddress = GetUiQueueName(DistributorDataAddress);
			//if we're a worker, send to the distributor data bus
			if (uiQueueAddress != null)
			{
				m.ReturnAddress = GetReturnAddressFor(uiQueueAddress, GetUiQueueName);

				transport.Send(m, uiQueueAddress);
			}
			else
			{
				m.ReturnAddress = GetReturnAddressFor(transport.Address, GetUiQueueName);

				transport.ReceiveMessageLater(m, GetUiQueueName(transport.Address));
			}
		}

		/// <summary>
		/// Sends the list of messages back to the current bus.
		/// </summary>
		/// <param name="messages">The messages to send.</param>
		public void SendLocal(params IMessage[] messages)
		{
			var m = GetTransportMessageFor(messages);

			//if we're a worker, send to the distributor data bus
			if (DistributorDataAddress != null)
			{
				m.ReturnAddress = GetReturnAddressFor(DistributorDataAddress, x => x);

				transport.Send(m, DistributorDataAddress);
			}
			else
			{
				m.ReturnAddress = GetReturnAddressFor(transport.Address, x => x);

				transport.ReceiveMessageLater(m);
			}
		}

		ICallback IBus.Send<T>(Action<T> messageConstructor)
		{
			return ((IBus) this).Send(CreateInstance(messageConstructor));
		}

		private string ProcessByProxy(string destination)
		{
			if (string.IsNullOrWhiteSpace(Proxy) || Proxy.Equals(destination, StringComparison.InvariantCultureIgnoreCase))
			{
				return destination;
			}

			if (((IBusExtended) this).OutgoingHeaders.ContainsKey("ReturnAddress"))
			{
				// Proxy is alreadfy initialized.
				return destination;
			}

			((IBusExtended) this).OutgoingHeaders.Add("ReturnAddress", destination);
			((IBusExtended) this).OutgoingHeaders.Add("ReturnAddressWasSetByProxy", null);

			if (IsUiQueue(destination))
			{
				return GetUiQueueName(Proxy);
			}

			return Proxy;
		}

		private void CleanUpProxyHeader()
		{
			if (((IBusExtended) this).OutgoingHeaders.ContainsKey("ReturnAddressWasSetByProxy"))
			{
				((IBusExtended) this).OutgoingHeaders.Remove("ReturnAddressWasSetByProxy");
				((IBusExtended) this).OutgoingHeaders.Remove("ReturnAddress");
			}
		}

		ICallback IBus.Send(params IMessage[] messages)
		{
			var destination = GetDestinationForMessageType(messages[0].GetType());

			return SendMessage(destination, null, MessageIntentEnum.Send, messages);
		}

		ICallback IBus.SendToUi(params IMessage[] messages)
		{
			var destination = GetDestinationForMessageType(messages[0].GetType());

			return SendMessage(GetUiQueueName(destination), null, MessageIntentEnum.Send, messages);
		}

		ICallback IBus.SendToUi(string destination, params IMessage[] messages)
		{
			return SendMessage(GetUiQueueName(destination), null, MessageIntentEnum.Send, messages);
		}

		ICallback IBus.Send<T>(string destination, Action<T> messageConstructor)
		{
			return SendMessage(destination, null, MessageIntentEnum.Send, CreateInstance(messageConstructor));
		}

		ICallback IBus.Send(string destination, params IMessage[] messages)
		{
			return SendMessage(destination, null, MessageIntentEnum.Send, messages);
		}

		ICallback IBus.Send<T>(string destination, string correlationId, Action<T> messageConstructor)
		{
			return SendMessage(destination, correlationId, MessageIntentEnum.Send, CreateInstance(messageConstructor));
		}

		ICallback IBus.Send(string destination, string correlationId, params IMessage[] messages)
		{
			return SendMessage(destination, correlationId, MessageIntentEnum.Send, messages);
		}


		private ICallback SendMessage(string destination, string correlationId, MessageIntentEnum messageIntent, params IMessage[] messages)
		{
			if (destination == null)
			{
				var tm = messages[0] as TimeoutMessage;
				if (tm != null && tm.ClearTimeout)
					return null;

				throw new InvalidOperationException(
					$"No destination specified for message {messages[0].GetType().FullName}. Message cannot be sent. " +
						"Check the UnicastBusConfig section in your config file and ensure that a MessageEndpointMapping exists for the message type.");
			}

			destination = ProcessByProxy(destination);
			foreach (var id in SendMessage(new List<string> { destination }, correlationId, messageIntent, messages))
			{
				if (Log.IsInfoEnabled)
				{
					string log = "Sent with ID {0} to {1}. Data: ".Fmt(id, destination);
					log += string.Join(", ", messages.Select(x => x.GetType().ToString()));

					Log.Info(log);
				}

				var result = new Callback(id);
				result.Registered += delegate(object sender, BusAsyncResultEventArgs args)
				{
					lock (_messageIdToAsyncResultLookup)
						_messageIdToAsyncResultLookup[args.MessageId] = args.Result;
				};
				CleanUpProxyHeader();
				return result;
			}

			CleanUpProxyHeader();
			return null;
		}

		private ICollection<string> SendMessage(IEnumerable<string> destinations, string correlationId, MessageIntentEnum messageIntent,
			params IMessage[] messages)
		{
			AssertBusIsStarted();

			var result = new List<string>();

			((IBus) this).OutgoingHeaders[EnclosedMessageTypes] = SerializeEnclosedMessageTypes(messages);
			var toSend = GetTransportMessageFor(messages);
			((IBus) this).OutgoingHeaders[EnclosedMessageTypes] = null;

			toSend.CorrelationId = correlationId;
			toSend.MessageIntent = messageIntent;

			foreach (var destination in destinations)
			{
				toSend.ReturnAddress = GetReturnAddressFor(destination, x => x);

				transport.Send(toSend, destination);

				if (Log.IsDebugEnabled)
					Log.Debug($"Sending message {messages[0].GetType().AssemblyQualifiedName} with ID {toSend.Id} to destination {destination}.\n"
						+ $"ToString() of the message yields: {messages[0]}\n"
						+ $"Message headers:\n{string.Join(", ", ((IBus) this).OutgoingHeaders.Select(h => h.Key + ":" + h.Value).ToArray())}");

				result.Add(toSend.Id);
			}

			return result;
		}

		/// <summary>
		/// Takes the given message types and serializes them for inclusion in the EnclosedMessageTypes header.
		/// </summary>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static string SerializeEnclosedMessageTypes(IMessage[] messages)
		{
			var types = GetFullTypes(messages);

			var sBuilder = new StringBuilder("<MessageTypes>");
			types.ForEach(s => sBuilder.Append("<s>" + s + "</s>"));
			sBuilder.Append("</MessageTypes>");

			return sBuilder.ToString();
		}

		/// <summary>
		/// Takes the serialized form of EnclosedMessageTypes and returns a list of string types.
		/// </summary>
		/// <param name="serialized"></param>
		/// <returns></returns>
		public static IList<string> DeserializeEnclosedMessageTypes(string serialized)
		{
			string temp = serialized.Replace("<MessageTypes><s>", "");
			temp = temp.Replace("</s></MessageTypes>", "");
			string[] arr = temp.Split(new[] { "</s><s>" }, StringSplitOptions.RemoveEmptyEntries);

			return new List<string>(arr);
		}

		private static List<string> GetFullTypes(IEnumerable<IMessage> messages)
		{
			var types = new List<string>();

			foreach (var m in messages)
			{
				var s = m.GetType().AssemblyQualifiedName;
				if (types.Contains(s))
					continue;

				types.Add(s);

				foreach (var t in m.GetType().GetInterfaces())
				{
					if (typeof(IMessage).IsAssignableFrom(t) && t != typeof(IMessage))
						if (!types.Contains(t.AssemblyQualifiedName))
							types.Add(t.AssemblyQualifiedName);
				}
			}

			return types;
		}

		/// <summary>
		/// Implementation of IStartableBus.Started event.
		/// </summary>
		public event EventHandler Started;

		IBus IStartableBus.Start()
		{
			return (this as IStartableBus).Start(null);
		}

		IBus IStartableBus.Start(Action startupAction)
		{
			if (_started)
				return this;

			lock (_startLocker)
			{
				if (_started)
					return this;

				_starting = true;

				startupAction?.Invoke();

				AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

				var mods = Builder.BuildAll<IMessageModule>();
				if (mods != null)
					modules.AddRange(mods);

				subscriptionStorage?.Init();

				transport.Start();

				if (_autoSubscribe)
				{
					foreach (var messageType in GetMessageTypesHandledOnThisEndpoint())
					{
						var destination = GetDestinationForMessageType(messageType);
						if (string.IsNullOrEmpty(destination))
							continue;

						var arr = destination.Split('@');

						var queue = arr[0];
						var machine = Environment.MachineName;

						if (arr.Length == 2)
							if (arr[1] != "." && arr[1].ToLower() != "localhost" && arr[1] != IPAddress.Loopback.ToString())
								machine = arr[1];

						destination = queue + "@" + machine;

						if (destination.ToLower() != transport.Address.ToLower())
							Subscribe(messageType);
					}
				}

				InitializeSelf();

				SendReadyMessage(true);

				_started = true;
			}

			Started?.Invoke(this, null);

			return this;
		}

		private void InitializeSelf()
		{
			var toSend = GetTransportMessageFor(new CompletionMessage());
			toSend.ReturnAddress = transport.Address;
			toSend.MessageIntent = MessageIntentEnum.Init;

			transport.ReceiveMessageLater(toSend);
		}

		/// <summary>
		/// If this bus is configured to feed off of a distributor,
		/// it will send a <see cref="ReadyMessage"/> to its control address.
		/// </summary>
		/// <param name="startup"></param>
		private void SendReadyMessage(bool startup)
		{
			if (DistributorControlAddress == null)
				return;

			if (!_canSendReadyMessages)
				return;

			IMessage[] messages;
			if (startup)
			{
				messages = new IMessage[transport.NumberOfWorkerThreads];
				for (var i = 0; i < transport.NumberOfWorkerThreads; i++)
				{
					var rm = new ReadyMessage
					{
						ClearPreviousFromThisAddress = (i == 0)
					};

					messages[i] = rm;
				}
			}
			else
			{
				messages = new IMessage[1];
				messages[0] = new ReadyMessage();
			}


			var toSend = GetTransportMessageFor(messages);
			toSend.ReturnAddress = transport.Address;

			transport.Send(toSend, DistributorControlAddress);

			Log.Debug("Sending ReadyMessage to " + DistributorControlAddress);
		}

		/// <summary>
		/// Tells the transport to dispose.
		/// </summary>
		public void Dispose()
		{
			Log.Info("Bus disposing");
			transport.Dispose();
			Log.Info("Bus disposed");
		}


		void IBus.DoNotContinueDispatchingCurrentMessageToHandlers()
		{
			_doNotContinueDispatchingCurrentMessageToHandlers = true;
		}

		[ThreadStatic] private static bool _doNotContinueDispatchingCurrentMessageToHandlers;

		[ThreadStatic] private static IDictionary<string, string> _outgoingHeaders = new Dictionary<string, string>();

		//private static IDictionary<string, string> OutgoingHeaders 

		IDictionary<string, string> IBus.OutgoingHeaders => _outgoingHeaders ?? (_outgoingHeaders = new Dictionary<string, string>());

		IMessageContext IBus.CurrentMessageContext => _messageBeingHandled == null ? null : new MessageContext(_messageBeingHandled);

		public const string SourceQueue = "SourceQueue";

		#endregion

		#region receiving and handling

		private void LogMessage(string message, Type messageType)
		{
			if (_messageTypesNotToLog.Contains(messageType))
			{
				Log.Debug(message);
			}
			else
			{
				Log.Info(message);
			}
		}

		/// <summary>
		/// Handles a received message.
		/// </summary>
		/// <param name="m">The received message.</param>
		/// <param name="messageType">message type</param>
		/// <remarks>
		/// run by multiple threads so must be thread safe
		/// public for testing
		/// </remarks>
		public void HandleMessage(TransportMessage m, Type messageType)
		{
			LogMessage("TpUnicastBus : begin calling handlers on message {0}".Fmt(m.Id), messageType);
			Thread.CurrentPrincipal = GetPrincipalToExecuteAs(m.WindowsIdentityName);

			CleanupOutgoingHeaders();

			ForwardMessageIfNecessary(m);

			HandleCorellatedMessage(m);

			foreach (var toHandle in m.Body)
			{
				ExtensionMethods.CurrentMessageBeingHandled = toHandle;

				var canDispatch = true;
				foreach (var condition in subscriptionsManager.GetConditionsForMessage(toHandle))
				{
					if (condition(toHandle)) continue;

					Log.Debug($"Condition {condition} failed for message {toHandle.GetType().Name}");
					canDispatch = false;
					break;
				}

				if (canDispatch)
					DispatchMessageToHandlersBasedOnType(toHandle, toHandle.GetType());
			}

			LogMessage("TpUnicastBus : end calling handlers on message {0}".Fmt(m.Id), messageType);

			ExtensionMethods.CurrentMessageBeingHandled = null;
			CleanupOutgoingHeaders();
		}

		public void CleanupOutgoingHeaders()
		{
			IBus bus = this;
			bus.OutgoingHeaders.Clear();
		}

		/// <summary>
		/// Called when unhandled exception from message handler caught.
		/// </summary>
		public event EventHandler<NServiceBus.Unicast.UnhandledExceptionEventArgs> UnhandledExceptionCaught;

		private void OnUnhandledExceptionCaught(Exception exception)
		{
			UnhandledExceptionCaught?.Invoke(this, new NServiceBus.Unicast.UnhandledExceptionEventArgs(exception));
		}

		/// <summary>
		/// Finds the message handlers associated with the message type and dispatches
		/// the message to the found handlers.
		/// </summary>
		/// <param name="toHandle">The message to dispatch to the handlers.</param>
		/// <param name="messageType">The message type by which to locate the correct handlers.</param>
		/// <returns></returns>
		/// <remarks>
		/// If during the dispatch, a message handler calls the DoNotContinueDispatchingCurrentMessageToHandlers method,
		/// this prevents the message from being further dispatched.
		/// This includes generic message handlers (of IMessage), and handlers for the specific messageType.
		/// </remarks>
		private void DispatchMessageToHandlersBasedOnType(IMessage toHandle, Type messageType)
		{
			foreach (var messageHandlerType in GetHandlerTypes(messageType))
			{
				try
				{
					Log.Debug("Activating: " + messageHandlerType.Name);

					Builder.BuildAndDispatch(messageHandlerType, GetAction(toHandle));

					Log.Debug(messageHandlerType.Name + " Done.");

					if (_doNotContinueDispatchingCurrentMessageToHandlers)
					{
						_doNotContinueDispatchingCurrentMessageToHandlers = false;
						return;
					}
				}
				catch (Exception e)
				{
					var innermostException = GetInnermostException(e);

					Log.Error(messageHandlerType.Name + " Failed handling message.", innermostException);

					OnUnhandledExceptionCaught(innermostException);

					throw;
				}
			}
		}

		private Action<object> GetAction<T>(T message) where T : IMessage
		{
			return (o =>
			{
				var messageTypesToMethods = _handlerToMessageTypeToHandleMethodMap[o.GetType()];
				foreach (var messageType in messageTypesToMethods.Keys)
				{
					if (messageType.IsInstanceOfType(message))
						messageTypesToMethods[messageType].Invoke(o, new object[] { message });
				}

				var disposable = o as IDisposable;
				disposable?.Dispose();
			}
				);
		}

		/// <summary>
		/// Gets the inner most exception from an exception stack.
		/// </summary>
		/// <param name="e">The exception to get the inner most exception for.</param>
		/// <returns>The innermost exception.</returns>
		/// <remarks>
		/// If the exception has no inner exceptions the provided exception will be returned.
		/// </remarks>
		private static Exception GetInnermostException(Exception e)
		{
			var result = e;
			while (result.InnerException != null)
			{
				result = result.InnerException;
			}

			return result;
		}

		/// <summary>
		/// If the message contains a correlationId, attempts to
		/// invoke callbacks for that Id.
		/// </summary>
		/// <param name="msg">The message to evaluate.</param>
		private void HandleCorellatedMessage(TransportMessage msg)
		{
			if (msg.CorrelationId == null)
				return;

			BusAsyncResult busAsyncResult;

			lock (_messageIdToAsyncResultLookup)
			{
				_messageIdToAsyncResultLookup.TryGetValue(msg.CorrelationId, out busAsyncResult);
				_messageIdToAsyncResultLookup.Remove(msg.CorrelationId);
			}

			if (busAsyncResult != null)
			{
				if (msg.Body?.Length == 1)
				{
					var cm = msg.Body[0] as CompletionMessage;
					if (cm != null)
						busAsyncResult.Complete(cm.ErrorCode, null);
					else
						busAsyncResult.Complete(int.MinValue, msg.Body);
				}
			}
		}

		/// <summary>
		/// Handles the <see cref="ITransport.TransportMessageReceived"/> event from the <see cref="ITransport"/> used
		/// for the bus.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The arguments for the event.</param>
		/// <remarks>
		/// When the transport passes up the <see cref="TransportMessage"/> its received,
		/// the bus checks for initializiation, 
		/// sets the message as that which is currently being handled for the current thread
		/// and, depending on <see cref="DisableMessageHandling"/>, attempts to handle the message.
		/// </remarks>
		private void TransportMessageReceived(object sender, TransportMessageReceivedEventArgs e)
		{
			var msg = e.Message;

			if (IsInitializationMessage(msg))
			{
				Log.Info(transport.Address + " initialized.");
				return;
			}

			if (HandledSubscriptionMessage(msg, subscriptionStorage, SubscriptionAuthorizer))
			{
				var messageType = GetSubscriptionMessageTypeFrom(msg);

				if (msg.MessageIntent == MessageIntentEnum.Subscribe)
				{
					ClientSubscribed?.Invoke(this, new SubscriptionEventArgs { MessageType = messageType, SubscriberAddress = msg.ReturnAddress });
				}

				return;
			}

			var message = "Received message " + msg.Body[0].GetType().Name + " with ID " + msg.Id + " from sender " + msg.ReturnAddress;
			LogMessage(message, msg.Body[0].GetType());

			_messageBeingHandled = msg;
			_handleCurrentMessageLaterWasCalled = false;

			MessageReceived?.Invoke(msg);

			if (!disableMessageHandling)
				HandleMessage(msg, msg.Body[0].GetType());

			Log.Debug("Finished handling message.");
		}

		private static string GetSubscriptionMessageTypeFrom(TransportMessage msg)
		{
			foreach (var header in msg.Headers)
			{
				if (header.Key == SubscriptionMessageType)
					return header.Value;
			}

			return null;
		}

		/// <summary>
		/// Handles subscribe and unsubscribe requests managing the given subscription storage.
		/// Returns true if the message was a subscription message.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="subscriptionStorage"></param>
		/// <param name="subscriptionAuthorizer"></param>
		/// <returns></returns>
		public static bool HandledSubscriptionMessage(TransportMessage msg, ISubscriptionStorage subscriptionStorage,
			IAuthorizeSubscriptions subscriptionAuthorizer)
		{
			string messageType = GetSubscriptionMessageTypeFrom(msg);

			Action warn = () =>
			{
				var warning =
					$"Subscription message from {msg.ReturnAddress} arrived at this endpoint, yet this endpoint is not configured to be a publisher.";

				Log.Warn(warning);

				if (Log.IsDebugEnabled) // only under debug, so that we don't expose ourselves to a denial of service
					throw new InvalidOperationException(warning); // and cause message to go to error queue by throwing an exception
			};

			if (msg.MessageIntent == MessageIntentEnum.Subscribe)
				if (subscriptionStorage != null)
				{
					bool goAhead = true;
					if (subscriptionAuthorizer != null)
						if (
							!subscriptionAuthorizer.AuthorizeSubscribe(messageType, msg.ReturnAddress, msg.WindowsIdentityName, new HeaderAdapter(msg.Headers)))
						{
							goAhead = false;
							Log.Info($"Subscription request from {msg.ReturnAddress} on message type {messageType} was refused.");
						}

					if (goAhead)
					{
						Log.Info("Subscribing " + msg.ReturnAddress + " to message type " + messageType);
						subscriptionStorage.Subscribe(msg.ReturnAddress, new[] { messageType });
					}

					return true;
				}
				else
				{
					warn();
				}

			if (msg.MessageIntent == MessageIntentEnum.Unsubscribe)
				if (subscriptionStorage != null)
				{
					bool goAhead = true;

					if (subscriptionAuthorizer != null)
						if (
							!subscriptionAuthorizer.AuthorizeUnsubscribe(messageType, msg.ReturnAddress, msg.WindowsIdentityName, new HeaderAdapter(msg.Headers)))
						{
							goAhead = false;
							Log.Debug($"Unsubscribe request from {msg.ReturnAddress} on message type {messageType} was refused.");
						}

					if (goAhead)
					{
						Log.Info("Unsubscribing " + msg.ReturnAddress + " from message type " + messageType);
						subscriptionStorage.Unsubscribe(msg.ReturnAddress, new[] { messageType });
					}

					return true;
				}
				else
				{
					warn();
				}

			return false;
		}

		private void TransportFinishedMessageProcessing(object sender, EventArgs e)
		{
			if (!_skipSendingReadyMessageOnce)
				SendReadyMessage(false);

			_skipSendingReadyMessageOnce = false;

			foreach (var module in modules)
			{
				Log.Debug("Calling 'HandleEndMessage' on " + module.GetType().FullName);
				module.HandleEndMessage();
			}
		}

		private void TransportFailedMessageProcessing(object sender, EventArgs e)
		{
			var exceptionThrown = false;

			foreach (var module in modules)
			{
				try
				{
					Log.Debug("Calling 'HandleError' on " + module.GetType().FullName);
					module.HandleError();
				}
				catch (Exception ex)
				{
					Log.Error("Module " + module.GetType().FullName + " failed when handling error.", ex);
					exceptionThrown = true;
				}
			}

			if (exceptionThrown)
				throw new Exception(
					"Could not handle the failed message processing correctly. Check for prior error messages in the log for more information.");
		}

		private void TransportStartedMessageProcessing(object sender, EventArgs e)
		{
			foreach (var module in modules)
			{
				Log.Debug("Calling 'HandleBeginMessage' on " + module.GetType().FullName);
				module.HandleBeginMessage(); //don't need to call others if one fails
			}
		}

		private bool IsInitializationMessage(TransportMessage msg)
		{
			if (msg.ReturnAddress == null)
				return false;

			if (!msg.ReturnAddress.Contains(transport.Address))
				return false;

			if (msg.CorrelationId != null)
				return false;

			if (msg.MessageIntent != MessageIntentEnum.Init)
				return false;

			if (msg.Body.Length > 1)
				return false;

			// A CompletionMessage is used out of convenience as the initialization message.
			var em = msg.Body[0] as CompletionMessage;
			return em != null;
		}

		#endregion

		#region helper methods

		/// <summary>
		/// Sets up known types needed for XML serialization as well as
		/// to which address to send which message.
		/// </summary>
		/// <param name="owners">A dictionary of message_type, address pairs.</param>
		private void ConfigureMessageOwners(IDictionary owners)
		{
			foreach (DictionaryEntry de in owners)
			{
				var key = de.Key as string;
				if (key == null)
					continue;

				try
				{
					var messageType = Type.GetType(key, false);
					if (messageType != null)
					{
						RegisterMessageType(messageType, de.Value as string, false);
						continue;
					}
				}
				catch (Exception ex)
				{
					Log.Error("Problem loading message type: " + key, ex);
				}

				try
				{
					var a = Assembly.Load(key);
					foreach (var t in a.GetTypes())
					{
						RegisterMessageType(t, de.Value.ToString(), true);
					}
				}
				catch (Exception ex)
				{
					throw new ArgumentException("Problem loading message assembly: " + key, ex);
				}
			}
		}

		/// <summary>
		/// Sends the Msg to the address found in the field <see cref="ForwardReceivedMessagesTo"/>
		/// if it isn't null.
		/// </summary>
		/// <param name="m">The message to forward</param>
		private void ForwardMessageIfNecessary(TransportMessage m)
		{
			if (ForwardReceivedMessagesTo != null)
				transport.Send(m, ForwardReceivedMessagesTo);
		}

		/// <summary>
		/// Registers a message type to a destination.
		/// </summary>
		/// <param name="messageType">A message type implementing <see cref="IMessage"/>.</param>
		/// <param name="destination">The address of the destination the message type is registered to.</param>
		/// <param name="configuredByAssembly">
		/// Indicates whether or not this registration call is related to a type configured from an
		/// assembly.
		/// </param>
		/// <remarks>
		/// Since the same message type may be configured specifically to one address
		/// and via its assembly to a different address, the configuredByAssembly
		/// parameter dictates that the specific message type data is to be used.
		/// </remarks>
		public void RegisterMessageType(Type messageType, string destination, bool configuredByAssembly)
		{
			if (typeof(IMessage) == messageType)
				return;

			if (typeof(IMessage).IsAssignableFrom(messageType))
			{
				if (MustNotOverrideExistingConfiguration(messageType, configuredByAssembly))
					return;

				_messageTypeToDestinationLookup[messageType] = destination;

				Log.Debug("Message " + messageType.FullName + " has been allocated to endpoint " + destination + ".");

				if (messageType.GetCustomAttributes(typeof(ExpressAttribute), true).Length == 0)
					_recoverableMessageTypes.Add(messageType);

				foreach (TimeToBeReceivedAttribute a in messageType.GetCustomAttributes(typeof(TimeToBeReceivedAttribute), true))
				{
					_timeToBeReceivedPerMessageType[messageType] = a.TimeToBeReceived;
				}
			}
		}

		/// <summary>
		/// Checks whether or not the existing configuration can be overridden for a message type.
		/// </summary>
		/// <param name="messageType">The type of message to check the configuration for.</param>
		/// <param name="configuredByAssembly">
		/// Indicates whether or not this check is related to a type configured from an
		/// assembly.
		/// </param>
		/// <returns>true if it is acceptable to override the configuration, otherwise false.</returns>
		private bool MustNotOverrideExistingConfiguration(Type messageType, bool configuredByAssembly)
		{
			return _messageTypeToDestinationLookup.ContainsKey(messageType) && configuredByAssembly;
		}

		/// <summary>
		/// Wraps the provided messages in an NServiceBus envelope, does not include destination.
		/// </summary>
		/// <param name="messages">The messages to wrap.</param>
		/// <returns>The envelope containing the messages.</returns>
		protected TransportMessage GetTransportMessageFor(params IMessage[] messages)
		{
			var result = new TransportMessage
			{
				Body = messages,
				WindowsIdentityName = Thread.CurrentPrincipal.Identity.Name
			};

			if (PropogateReturnAddressOnSend)
				result.ReturnAddress = transport.Address;

			result.Headers = HeaderAdapter.From(_outgoingHeaders);

			var timeToBeReceived = TimeSpan.MaxValue;

			foreach (var message in messages)
			{
				if (_recoverableMessageTypes.Contains(message.GetType()))
					result.Recoverable = true;

				var span = GetTimeToBeReceivedForMessageType(message.GetType());
				timeToBeReceived = (span < timeToBeReceived ? span : timeToBeReceived);
			}

			result.TimeToBeReceived = timeToBeReceived;

			return result;
		}

		private TimeSpan GetTimeToBeReceivedForMessageType(Type messageType)
		{
			var result = TimeSpan.MaxValue;

			_timeToBeReceivedPerMessageTypeLocker.EnterReadLock();
			if (_timeToBeReceivedPerMessageType.ContainsKey(messageType))
			{
				result = _timeToBeReceivedPerMessageType[messageType];
				_timeToBeReceivedPerMessageTypeLocker.ExitReadLock();
				return result;
			}

			var options = new List<TimeSpan>();
			foreach (var interfaceType in messageType.GetInterfaces())
			{
				if (_timeToBeReceivedPerMessageType.ContainsKey(interfaceType))
					options.Add(_timeToBeReceivedPerMessageType[interfaceType]);
			}

			_timeToBeReceivedPerMessageTypeLocker.ExitReadLock();

			if (options.Count > 0)
				result = options.Min();

			_timeToBeReceivedPerMessageTypeLocker.EnterWriteLock();
			_timeToBeReceivedPerMessageType[messageType] = result;
			_timeToBeReceivedPerMessageTypeLocker.ExitWriteLock();

			return result;
		}

		private string GetReturnAddressFor(string destination, Func<string, string> getAddress)
		{
			var result = getAddress(transport.Address);

			// if we're a worker
			if (getAddress(DistributorDataAddress) != null)
			{
				result = getAddress(DistributorDataAddress);

				//if we're sending a message to the control bus, then use our own address
				if (destination == getAddress(DistributorControlAddress))
					result = getAddress(transport.Address);
			}

			return result;
		}

		private void IfTypeIsSagaMessageHandlerThenLoad(Type t)
		{
			if (t.IsAbstract)
				return;

			// Changed by TargetProcess.
			// We add saga handlers to special collection. And we use it to subscribe to all messages handled by sagas. So we filter messages that plugin will receive.
			if (typeof(ISaga).IsAssignableFrom(t))
			{
				foreach (var messageType in GetMessageTypesIfIsMessageHandler(t))
				{
					if (!_sagaHandlerList.ContainsKey(t))
						_sagaHandlerList.Add(t, new List<Type>());

					if (!(_sagaHandlerList[t].Contains(messageType)))
					{
						_sagaHandlerList[t].Add(messageType);
						Log.Debug($"Associated '{messageType}' message with '{t}' saga handler");
					}
				}
			}
		}

		/// <summary>
		/// Evaluates a type and loads it if it implements IMessageHander{T}.
		/// </summary>
		/// <param name="t">The type to evaluate.</param>
		private void IfTypeIsMessageHandlerThenLoad(Type t)
		{
			if (t.IsAbstract)
				return;

			foreach (var messageType in GetMessageTypesIfIsMessageHandler(t))
			{
				if (!_handlerList.ContainsKey(t))
					_handlerList.Add(t, new List<Type>());

				if (!(_handlerList[t].Contains(messageType)))
				{
					_handlerList[t].Add(messageType);
					Log.Debug($"Associated '{messageType}' message with '{t}' handler");
				}

				if (!_handlerToMessageTypeToHandleMethodMap.ContainsKey(t))
					_handlerToMessageTypeToHandleMethodMap.Add(t, new Dictionary<Type, MethodInfo>());

				if (!(_handlerToMessageTypeToHandleMethodMap[t].ContainsKey(messageType)))
					_handlerToMessageTypeToHandleMethodMap[t].Add(messageType, t.GetMethod("Handle", new[] { messageType }));
			}
		}

		/// <summary>
		/// If the type is a message handler, returns all the message types that it handles
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IEnumerable<Type> GetMessageTypesIfIsMessageHandler(Type type)
		{
			foreach (var t in type.GetInterfaces())
			{
				if (t.IsGenericType)
				{
					var args = t.GetGenericArguments();
					if (args.Length != 1)
						continue;

					if (!typeof(IMessage).IsAssignableFrom(args[0]))
						continue;

					var handlerType = typeof(IMessageHandler<>).MakeGenericType(args[0]);
					if (handlerType.IsAssignableFrom(t))
						yield return args[0];
				}
			}
		}

		/// <summary>
		/// Gets a list of handler types associated with a message type.
		/// </summary>
		/// <param name="messageType">The type of message to get the handlers for.</param>
		/// <returns>The list of handler types associated with the message type.</returns>
		private IEnumerable<Type> GetHandlerTypes(Type messageType)
		{
			foreach (var handlerType in _handlerList.Keys)
			{
				foreach (var msgTypeHandled in _handlerList[handlerType])
				{
					if (msgTypeHandled.IsAssignableFrom(messageType))
					{
						yield return handlerType;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Returns all the message types which have handlers configured for them.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Type> GetMessageTypesHandledOnThisEndpoint()
		{
			foreach (var handlerType in _handlerList.Keys)
			{
				foreach (var msgTypeHandled in _handlerList[handlerType])
				{
					yield return msgTypeHandled;
				}
			}

			foreach (var handlerType in _sagaHandlerList.Keys)
			{
				foreach (var msgTypeHandled in _sagaHandlerList[handlerType])
				{
					yield return msgTypeHandled;
				}
			}
		}

		/// <summary>
		/// Gets the destination address for a message type.
		/// </summary>
		/// <param name="messageType">The message type to get the destination for.</param>
		/// <returns>The address of the destination associated with the message type.</returns>
		protected string GetDestinationForMessageType(Type messageType)
		{
			string destination;

			lock (_messageTypeToDestinationLookup)
			{
				if (!_messageTypeToDestinationLookup.TryGetValue(messageType, out destination))
				{
					if (messageType.IsGenericType)
					{
						_messageTypeToDestinationLookup.TryGetValue(messageType.GetGenericTypeDefinition(), out destination);
					}
				}
			}

			if (destination == string.Empty)
				destination = null;

			if (destination == null)
			{
				if (messageType.IsInterface)
					return null;

				var t = messageMapper?.GetMappedTypeFor(messageType);
				if (t != null && t != messageType)
					return GetDestinationForMessageType(t);
			}

			return destination;
		}

		/// <summary>
		/// Throws an exception if the bus hasn't begun the startup process.
		/// </summary>
		protected void AssertBusIsStarted()
		{
			if (_starting == false)
				throw new InvalidOperationException("The bus is not started yet, call Bus.Start() before attempting to use the bus.");
		}

		IPrincipal GetPrincipalToExecuteAs(string windowsIdentityName)
		{
			if (!ImpersonateSender)
				return null;

			if (string.IsNullOrEmpty(windowsIdentityName))
			{
				Log.Info("Can't impersonate because no windows identity specified in incoming message. This is common in interop scenarios.");
				return null;
			}

			return new GenericPrincipal(new GenericIdentity(windowsIdentityName), new string[0]);
		}

		#endregion

		#region Fields

		/// <summary>
		/// Gets/sets the subscription manager to use for the bus.
		/// </summary>
		protected SubscriptionsManager subscriptionsManager = new SubscriptionsManager();

		/// <summary>
		/// Gets/sets the subscription storage.
		/// </summary>
		protected ISubscriptionStorage subscriptionStorage;

		/// <summary>
		/// The list of message modules.
		/// </summary>
		protected readonly List<IMessageModule> modules = new List<IMessageModule>();

		private readonly IDictionary<Type, List<Type>> _handlerList = new Dictionary<Type, List<Type>>();
		private readonly IDictionary<Type, List<Type>> _sagaHandlerList = new Dictionary<Type, List<Type>>();

		private readonly IDictionary<Type, IDictionary<Type, MethodInfo>> _handlerToMessageTypeToHandleMethodMap =
			new Dictionary<Type, IDictionary<Type, MethodInfo>>();

		private readonly IDictionary<string, BusAsyncResult> _messageIdToAsyncResultLookup = new Dictionary<string, BusAsyncResult>();
		private readonly IList<Type> _recoverableMessageTypes = new List<Type>();

		private readonly IDictionary<Type, TimeSpan> _timeToBeReceivedPerMessageType = new Dictionary<Type, TimeSpan>();
		private readonly ReaderWriterLockSlim _timeToBeReceivedPerMessageTypeLocker = new ReaderWriterLockSlim();

		/// <remarks>
		/// Accessed by multiple threads - needs appropriate locking
		/// </remarks>
		private readonly IDictionary<Type, string> _messageTypeToDestinationLookup = new Dictionary<Type, string>();

		/// <remarks>
		/// ThreadStatic
		/// </remarks>
		[ThreadStatic] private static TransportMessage _messageBeingHandled;

		/// <summary>
		/// Accessed by multiple threads.
		/// </summary>
		private volatile bool _canSendReadyMessages = true;

		/// <summary>
		/// ThreadStatic
		/// </summary>
		[ThreadStatic] private static bool _skipSendingReadyMessageOnce;

		private volatile bool _started;
		private volatile bool _starting;
		private readonly object _startLocker = new object();

		private static readonly ILog Log = LogManager.GetLogger(typeof(TpUnicastBus));

		#endregion
	}
}
