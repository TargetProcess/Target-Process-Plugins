// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Transactions;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.ObjectBuilder;
using NServiceBus.Unicast.Config;
using NServiceBus.Unicast.Transport;
using StructureMap;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.ServiceBus.Serialization;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router;
using Tp.Integration.Messages.ServiceBus.UnicastBus;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Testing.Common.Persisters;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.Integration.Testing.Common
{
	public class TransportMock : IMsmqTransport
	{
		#region ITransport Members

		/// <summary>
		/// Event which indicates that message processing has started.
		/// </summary>
		public event EventHandler StartedMessageProcessing;

		/// <summary>
		/// Event which indicates that message processing has completed.
		/// </summary>
		public event EventHandler FinishedMessageProcessing;

		/// <summary>
		/// Event which indicates that message processing failed for some reason.
		/// </summary>
		public event EventHandler FailedMessageProcessing
		{
			add { }
			remove { }
		}

		/// <summary>
		/// Gets/sets the number of concurrent threads that should be
		/// created for processing the queue.
		/// 
		/// Get returns the actual number of running worker threads, which may
		/// be different than the originally configured value.
		/// 
		/// When used as a setter, this value will be used by the <see cref="Start"/>
		/// method only and will have no effect if called afterwards.
		/// 
		/// To change the number of worker threads at runtime, call <see cref="ChangeNumberOfWorkerThreads"/>.
		/// </summary>
		public virtual int NumberOfWorkerThreads { get; set; }


		/// <summary>
		/// Event raised when a message has been received in the input queue.
		/// </summary>
		public event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;

		/// <summary>
		/// Gets the address of the input queue.
		/// </summary>
		public string Address
		{
			get { return INPUT_QUEUE_NAME; }
		}

		/// <summary>
		/// Changes the number of worker threads to the given target,
		/// stopping or starting worker threads as needed.
		/// </summary>
		/// <param name="targetNumberOfWorkerThreads"></param>
		public void ChangeNumberOfWorkerThreads(int targetNumberOfWorkerThreads)
		{
			NumberOfWorkerThreads = targetNumberOfWorkerThreads;
		}

		/// <summary>
		/// Starts the transport.
		/// </summary>
		public void Start() {}

		private readonly MessageQueue _queueToTp = new MessageQueue(TP_INPUT_COMMAND_QUEUE_NAME);
		private readonly MessageQueue _sentToLocalQueue = new MessageQueue(INPUT_QUEUE_NAME);
		private readonly MessageQueue _queueWithPresetupAnswers = new MessageQueue("QueueWithPresetupAnswers");


		/// <summary>
		/// Re-queues a message for processing at another time.
		/// </summary>
		/// <param name="m">The message to process later.</param>
		/// <remarks>
		/// This method will place the message onto the back of the queue
		/// which may break message ordering.
		/// </remarks>
		public void ReceiveMessageLater(TransportMessage m)
		{
			var sentMessage = new SentMessage { Destination = Address, Message = m };
			_queueToReceiveMessagesLater.Enqueue(sentMessage);
			LocalQueue.Enqueue(sentMessage);
		}

		private readonly MessageQueue _queueToReceiveMessagesLater = new MessageQueue(INPUT_QUEUE_NAME);
		private const string INPUT_QUEUE_NAME = "Input.Queue.Mock.Address";
		private const string TP_INPUT_COMMAND_QUEUE_NAME = "Tp.InputCommand";

		/// <summary>
		/// Sends a message to the specified destination.
		/// </summary>
		/// <param name="m">The message to send.</param>
		/// <param name="destination">The address of the destination to send the message to.</param>
		public void Send(TransportMessage m, string destination)
		{
			if (destination == Address)
			{
				ReceiveMessageLater(m);
			}
			else
			{
				if (!m.Body.Any(x => x.GetType().Assembly == typeof (TransportMessage).Assembly))
				{
					TpQueue.Enqueue(new SentMessage {Destination = destination, Message = m});
				}
			}

			var t = m.Clone<TransportMessage>();
			t.ReturnAddress = destination;
			_queueWithPresetupAnswers.Enqueue(new SentMessage {Destination = destination, Message = t});
		}

		/// <summary>
		/// Returns the number of messages in the queue.
		/// </summary>
		/// <returns></returns>
		public bool QueueIsNotEmpty()
		{
			return TpQueue.Count > 0;
		}

		#endregion

		#region helper methods

		/// <summary>
		/// Causes the processing of the current message to be aborted.
		/// </summary>
		public void AbortHandlingCurrentMessage() {}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Stops all worker threads and disposes the MSMQ queue.
		/// </summary>
		public void Dispose() {}

		#endregion

		public void HandleLocalMessage(IProfileReadonly profile, params IMessage[] messages)
		{
			HandleMessageFromQueue(messages, profile, Address);
		}

		public void HandleLocalMessage(List<HeaderInfo> headers, params IMessage[] messages)
		{
			HandleMessageFromQueue(messages, headers, Address);
		}

		public void HandleMessageFromTp(IProfileReadonly profile, params IMessage[] messages)
		{
			HandleMessageFromQueue(messages, profile, TP_INPUT_COMMAND_QUEUE_NAME);
		}

		private void HandleMessageFromQueue(IMessage[] messages, IProfileReadonly profile, string queueName)
		{
			var headers = new List<HeaderInfo> {new HeaderInfo {Key = BusExtensions.PROFILENAME_KEY, Value = profile.Name.Value}};
			HandleMessageFromQueue(messages, headers, queueName);
		}

		public void HandleMessageFromTp(IMessage message)
		{
			HandleMessageFromTp(new List<HeaderInfo>(), message);
		}

		public void HandleMessageFromTp(List<HeaderInfo> headers, params IMessage[] messages)
		{
			HandleMessageFromQueue(messages, headers, TP_INPUT_COMMAND_QUEUE_NAME);
		}

		private void HandleMessageFromQueue(IMessage[] messages, List<HeaderInfo> headers, string queueName)
		{
			if (!messages.Any())
			{
				return;
			}

			var transportMessage = new TransportMessage
			                       	{
			                       		Body = messages,
			                       		WindowsIdentityName = Thread.CurrentPrincipal.Identity.Name,
			                       		Headers = headers,
			                       		ReturnAddress = queueName
			                       	};

			var sentMessage = new SentMessage {Destination = Address, Message = transportMessage};
			MessageReceived(sentMessage.Message);
			LocalQueue.Enqueue(sentMessage);

			while (_queueToReceiveMessagesLater.Count > 0)
			{
				MessageReceived(_queueToReceiveMessagesLater.Dequeue().Message);
			}
		}

		public MessageQueue LocalQueue
		{
			get { return _sentToLocalQueue; }
		}

		public MessageQueue TpQueue
		{
			get { return _queueToTp; }
		}

		private void MessageReceived(TransportMessage transportMessage)
		{
			StartedMessageProcessing.Raise(this, EventArgs.Empty);
			TransportMessageReceived.Raise(this, new TransportMessageReceivedEventArgs(transportMessage));
			FinishedMessageProcessing.Raise(this, EventArgs.Empty);

			RaiseTransportMessageReceived();
		}

		private static TransportMock Create(Action configureStructureMap, IEnumerable<Assembly> assembliesToScan,
		                                    params Assembly[] messageAssemblies)
		{
			configureStructureMap();
			var assemblies = new List<Assembly>(assembliesToScan)
			                 	{
			                 		PluginAssembly,
			                 		TpIntegrationMessagesAssembly,
			                 		TpIntegrationPluginCommonAssembly,
			                 		NServiceBusCoreAssembly,
			                 		NServiceBusAssembly
			                 	};

				Configure.With(assemblies)
					.CustomConfigurationSource(new IntegrationTestConfigurationSource(messageAssemblies))
					.StructureMapBuilder(ObjectFactory.Container)
					.AdvancedXmlSerializer().TransportMock().Sagas().TpInMemorySagaPersister().TpUnicastBus().
				LoadMessageHandlers(GetHandlersOrder()).CreateBus().Start();

			return ObjectFactory.GetInstance<TransportMock>();
		}

		private static First<PluginGateway> GetHandlersOrder()
		{
			var ordering = First<PluginGateway>.Then<PluginGateway>();
			var messageHandlerOrdering = ObjectFactory.TryGetInstance<ICustomPluginSpecifyMessageHandlerOrdering>();
			if (messageHandlerOrdering != null)
			{
				messageHandlerOrdering.SpecifyHandlersOrder(ordering);
			}

			return ordering;
		}

		[Obsolete(
			"This method initializes StructureMap from scratch, so it's preferable to use CreateWithoutStructureMapClear")]
		public static TransportMock Create(Assembly pluginAssembly, params Assembly[] messageAssemblies)
		{
			return Create(() => ObjectFactory.Initialize(x => x.AddRegistry(new AssemblyScannerMockRegistry(pluginAssembly))),
			              new List<Assembly>(),
			              messageAssemblies);
		}

		public static TransportMock CreateWithoutStructureMapClear(Assembly pluginAssembly,
		                                                           params Assembly[] messageAssemblies)
		{
			return CreateWithoutStructureMapClear(pluginAssembly, new Assembly[] {}, messageAssemblies);
		}

		public static TransportMock CreateWithoutStructureMapClear(Assembly pluginAssembly,
		                                                           IEnumerable<Assembly> assembliesToScan,
		                                                           params Assembly[] messageAssemblies)
		{
			return Create(() => ObjectFactory.Configure(x =>
			                                            	{
			                                            		x.Scan(y =>
			                                            		       	{
			                                            		       		assembliesToScan.ForEach(y.Assembly);
			                                            		       		y.LookForRegistries();
			                                            		       	});
			                                            		x.AddRegistry(new AssemblyScannerMockRegistry(pluginAssembly));
			                                            	}),
			              assembliesToScan, messageAssemblies);
		}

		private static Assembly PluginAssembly
		{
			get { return ObjectFactory.GetInstance<IAssembliesHost>().GetAssemblies().Single(); }
		}

		private static Assembly NServiceBusAssembly
		{
			get { return typeof (CompletionResult).Assembly; }
		}

		private static Assembly NServiceBusCoreAssembly
		{
			get { return typeof (Configure).Assembly; }
		}

		private static Assembly TpIntegrationPluginCommonAssembly
		{
			get { return typeof (PluginGateway).Assembly; }
		}

		private static Assembly TpIntegrationMessagesAssembly
		{
			get { return typeof (UserStoryCreatedMessage).Assembly; }
		}

		private class IntegrationTestConfigurationSource : PluginEndpoint.PluginConfigurationSource
		{
			private readonly Assembly[] _messageAssemblies;

			public IntegrationTestConfigurationSource(Assembly[] messageAssemblies)
			{
				_messageAssemblies = messageAssemblies;
			}

			protected override IEnumerable<MessageEndpointMapping> GetExtraMappings()
			{
				return
					_messageAssemblies.Select(
						assembly => new MessageEndpointMapping {Messages = assembly.FullName, Endpoint = TargetProcessInputQueue});
			}
		}

		/// <summary>
		/// Adds a profile for default account. Profile is not supposed to implement IValidatable interface
		/// </summary>
		/// <param name="profileName"></param>
		/// <returns></returns>
		public IProfile AddProfile(ProfileName profileName)
		{
			return AddProfile(profileName, AccountName.Empty);
		}

		/// <summary>
		/// Adds a profile for default account
		/// </summary>
		/// <param name="profileName"></param>
		/// <param name="profileSettings"></param>
		/// <returns></returns>
		public IProfile AddProfile(ProfileName profileName, object profileSettings)
		{
			return AddProfile(profileName, AccountName.Empty, profileSettings);
		}

		/// <summary>
		/// Adds a profile for an account
		/// </summary>
		/// <param name="profileName"></param>
		/// <param name="accountName"></param>
		/// <param name="profileSettings"></param>
		/// <returns></returns>
		public IProfile AddProfile(ProfileName profileName, AccountName accountName, object profileSettings)
		{
			var command = new ExecutePluginCommandCommand
			              	{
			              		CommandName = EmbeddedPluginCommands.AddProfile,
			              		Arguments = new PluginProfileDto {Name = profileName.Value, Settings = profileSettings}.Serialize()
			              	};

			HandleMessageFromTp(
				new List<HeaderInfo> {new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = accountName.Value}}, command);

			var account = ObjectFactory.GetInstance<IAccountCollection>().GetOrCreate(accountName);
			var profile = account.Profiles[profileName];

			if (profile == null)
			{
				throw new ApplicationException(
					string.Format(
						"Profile '{0}' was not found in account '{1}'. Probably it's because profile did not pass validation -- check TpQueue for errors",
						profileName.Value, accountName.Value));
			}

			return profile;
		}

		/// <summary>
		/// Adds a profile for an account. Profile is not supposed to implement IValidatable interface
		/// </summary>
		/// <param name="profileName"></param>
		/// <param name="accountName"></param>
		/// <returns></returns>
		public IProfile AddProfile(ProfileName profileName, AccountName accountName)
		{
			return AddProfile(profileName, accountName,
			                  Activator.CreateInstance(ObjectFactory.GetInstance<IPluginMetadata>().ProfileType));
		}

		public IOnMessageHandler<T> On<T>() where T : IMessage
		{
			var onMessageHandler = new OnMessageHandler<T>(this);
			_onMessageHandlers.Add(onMessageHandler);
			return onMessageHandler;
		}

		public IOnMessageHandler<T> On<T>(Func<T, bool> canBeHandled) where T : IMessage
		{
			var onMessageHandler = new OnMessageHandler<T>(this, canBeHandled);
			_onMessageHandlers.Add(onMessageHandler);
			return onMessageHandler;
		}

		public void ResetAllOnMessageHandlers()
		{
			_onMessageHandlers.ForEach(x => x.Dispose());
			_onMessageHandlers.Clear();
		}

		public List<IDisposable> _onMessageHandlers = new List<IDisposable>();
		public event EventHandler<TransportMessageReceivedEventArgs> PluginMessageSent;

		public void RaiseTransportMessageReceived()
		{
			while (_queueWithPresetupAnswers.Count > 0)
			{
				var message = _queueWithPresetupAnswers.Dequeue();
				PluginMessageSent.Raise(this, new TransportMessageReceivedEventArgs(message.Message));
			}
		}

		private readonly Dictionary<Type, object> _createCommandExpectations = new Dictionary<Type, object>();
		private readonly Dictionary<Type, object> _updateCommandExpectations = new Dictionary<Type, object>();
		private readonly Dictionary<Type, object> _deleteCommandExpectations = new Dictionary<Type, object>();

		public CreateCommandExpectations<TDto> OnCreateEntityCommand<TDto>() where TDto : DataTransferObject, new()
		{
			return OnCommand<CreateCommandExpectations<TDto>, TDto>(_createCommandExpectations);
		}

		public UpdateCommandExpectations<TDto> OnUpdateEntityCommand<TDto>() where TDto : DataTransferObject, new()
		{
			return OnCommand<UpdateCommandExpectations<TDto>, TDto>(_updateCommandExpectations);
		}

		public DeleteCommandExpectations<TDto> OnDeleteEntityCommand<TDto>() where TDto : DataTransferObject, new()
		{
			return OnCommand<DeleteCommandExpectations<TDto>, TDto>(_deleteCommandExpectations);
		}

		private TExpectations OnCommand<TExpectations, TDto>(IDictionary<Type, object> expectations)
			where TDto : DataTransferObject, new() where TExpectations : CommandExpectationBase<TDto>, new()
		{
			if (!expectations.ContainsKey(typeof (TDto)))
			{
				var expectation = new TExpectations();
				expectation.InitTransport(this);

				expectations[typeof (TDto)] = expectation;
			}
			return expectations[typeof (TDto)] as TExpectations;
		}

		public string InputQueue { get; set; }
		public string ErrorQueue { get; set; }
		public int MaxRetries { get; set; }
		public IPluginQueueFactory PluginQueueFactory { get; set; }
		public bool IsTransactional { get; set; }
		public TimeSpan TransactionTimeout { get; set; }
		public IsolationLevel IsolationLevel { get; set; }
		public bool DoNotCreateQueues { get; set; }
		public bool PurgeOnStartup { get; set; }
		public RoutableTransportMode RoutableTransportMode { get; set; }
		public void ReceiveMessageLater(TransportMessage m, string address)
		{
		}
	}

	internal static class ConfigureInMemorySagaPersister
	{
		public static Configure TpInMemorySagaPersister(this Configure config)
		{
			config.Configurer.ConfigureComponent(typeof (TpInMemorySagaPersister), ComponentCallModelEnum.Singleton);
			return config;
		}
	}
}
