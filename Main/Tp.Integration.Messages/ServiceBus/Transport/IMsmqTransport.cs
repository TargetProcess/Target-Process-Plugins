// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Transactions;
using NServiceBus.Unicast.Transport;
using Tp.Integration.Messages.ServiceBus.Transport.Router;

namespace Tp.Integration.Messages.ServiceBus.Transport
{
	public interface IMsmqTransport : ITransport
	{
		/// <summary>
		/// The path to the queue the transport will read from.
		/// Only specify the name of the queue - msmq specific address not required.
		/// When using MSMQ v3, only local queues are supported.
		/// </summary>
		string InputQueue { get; set; }

		/// <summary>
		/// Sets the path to the queue the transport will transfer
		/// errors to.
		/// </summary>
		string ErrorQueue { get; set; }

		/// <summary>
		/// Sets the maximum number of times a message will be retried
		/// when an exception is thrown as a result of handling the message.
		/// This value is only relevant when <see cref="IsTransactional"/> is true.
		/// </summary>
		/// <remarks>
		/// Default value is 5.
		/// </remarks>
		int MaxRetries { get; set; }

		IPluginQueueFactory PluginQueueFactory { get; set; }

		/// <summary>
		/// Sets whether or not the transport is transactional.
		/// </summary>
		bool IsTransactional { get; set; }

		/// <summary>
		/// Property for getting/setting the period of time when the transaction times out.
		/// Only relevant when <see cref="IsTransactional"/> is set to true.
		/// </summary>
		TimeSpan TransactionTimeout { get; set; }

		/// <summary>
		/// Property for getting/setting the isolation level of the transaction scope.
		/// Only relevant when <see cref="IsTransactional"/> is set to true.
		/// </summary>
		IsolationLevel IsolationLevel { get; set; }

		/// <summary>
		/// Property indicating that queues will not be created on startup
		/// if they do not already exist.
		/// </summary>
		bool DoNotCreateQueues { get; set; }

		/// <summary>
		/// Sets whether or not the transport should purge the input
		/// queue when it is started.
		/// </summary>
		bool PurgeOnStartup { get; set; }

		RoutableTransportMode RoutableTransportMode { get; set; }

		void ReceiveMessageLater(TransportMessage m, string address);

		string GetQueueName(string accountName);

		bool TryDeleteQueue(string accountName);
		bool TryDeleteUiQueue(string accountName);
	}
}