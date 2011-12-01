// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage;
using ISagaMessage = Tp.Integration.Messages.EntityLifecycle.ISagaMessage;

namespace Tp.Integration.Plugin.Common
{
	/// <summary>
	/// This class is used to define sagas containing data and handling a message.
	/// To handle more message types, implement <see cref="IMessageHandler{T}"/>
	/// for the relevant types.
	/// To signify that the receipt of a message should start this saga,
	/// implement <see cref="ISagaStartedBy{T}"/> for the relevant message type.
	/// For sending messages from saga please use methods of this class.
	/// </summary>
	/// <typeparam name="TSagaData">A type that implements <see cref="ISagaEntity"/>.</typeparam>
	public class TpSaga<TSagaData> : Saga<TSagaData> where TSagaData : ISagaEntity
	{
		public TpSaga()
		{
			StorageRepository = () =>
			                    {
			                    	var storage = ObjectFactory.GetInstance<IStorageRepository>();
			                    	StorageRepository = () => storage;
			                    	return storage;
			                    };

			Log = () =>
			      {
			      	var log = ObjectFactory.GetInstance<IActivityLogger>();
			      	Log = () => log;
			      	return log;
			      };

			_tpBus = () =>
			         {
			         	var tpBus = ObjectFactory.GetInstance<ITpBus>();
			         	_tpBus = () => tpBus;
			         	return tpBus;
			         };
		}

		/// <summary>
		/// Functor to access StorageRepository.
		/// </summary>
		public Func<IStorageRepository> StorageRepository { get; private set; }

		/// <summary>
		/// Functor to access Log.
		/// </summary>
		public Func<IActivityLogger> Log { get; set; }

		private Func<ITpBus> _tpBus;

		/// <summary>
		/// Checks if the message is a part of some saga.
		/// </summary>
		/// <param name="message">The message to check.</param>
		/// <returns>True, if the message is a part of some saga.</returns>
		protected static bool IsResponse(ISagaMessage message)
		{
			return ObjectFactory.GetInstance<ISagaPersister>().Get<ISagaEntity>(message.SagaId) != null;
		}

		/// <summary>
		/// Sends messages to plugin queue.
		/// </summary>
		/// <param name="localMessages">Messages to send.</param>
		protected void SendLocal(params IPluginLocalMessage[] localMessages)
		{
			_tpBus().SendLocal(localMessages);
		}

		/// <summary>
		/// Sends message to TargetProcess.
		/// </summary>
		/// <param name="messages">Messages to send.</param>
		/// <returns>Objects of this interface are returned from calling IBus.Send. The interface allows the caller to register for a callback when a response is received to their original call to IBus.Send.</returns>
		protected ICallback Send(params ITargetProcessMessage[] messages)
		{
			_tpBus().SetOutSagaId(Data.Id);
			return _tpBus().Send(messages);
		}

		/// <summary>
		/// Sends create commands to TargetProcess.
		/// </summary>
		/// <param name="createCommands">Create commands to send.</param>
		/// <returns>Objects of this interface are returned from calling IBus.Send. The interface allows the caller to register for a callback when a response is received to their original call to IBus.Send.</returns>
		protected ICallback Send(params ICreateEntityCommand<DataTransferObject>[] createCommands)
		{
			_tpBus().SetOutSagaId(Data.Id);
			return _tpBus().Send(createCommands);
		}

		/// <summary>
		/// Sends update commands to TargetProcess.
		/// </summary>
		/// <param name="updateCommands">Update commands to send.</param>
		/// <returns>Objects of this interface are returned from calling IBus.Send. The interface allows the caller to register for a callback when a response is received to their original call to IBus.Send.</returns>
		protected ICallback Send(params IUpdateEntityCommand<DataTransferObject>[] updateCommands)
		{
			_tpBus().SetOutSagaId(Data.Id);
			return _tpBus().Send(updateCommands);
		}

		/// <summary>
		/// Sends delete commands to TargetProcess
		/// </summary>
		/// <param name="deleteCommands">Delete commands to send.</param>
		/// <returns>Objects of this interface are returned from calling IBus.Send. The interface allows the caller to register for a callback when a response is received to their original call to IBus.Send.</returns>
		protected ICallback Send(params IDeleteEntityCommand[] deleteCommands)
		{
			_tpBus().SetOutSagaId(Data.Id);
			return _tpBus().Send(deleteCommands);
		}

		/// <summary>
		/// Tells the bus to stop dispatching the current message to additional
		/// handlers.
		/// </summary>
		protected void DoNotContinueDispatchingCurrentMessageToHandlers()
		{
			_tpBus().DoNotContinueDispatchingCurrentMessageToHandlers();
		}
	}
}