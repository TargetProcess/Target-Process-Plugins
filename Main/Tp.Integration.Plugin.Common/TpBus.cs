// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Linq;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.ServiceBus.UnicastBus;

namespace Tp.Integration.Plugin.Common
{
	public class TpBus : ITpBus
	{
		private readonly IBusExtended _bus;

		public TpBus(IBusExtended bus)
		{
			_bus = bus;
		}

		#region Entity Commands

		public ICallback Send(params ICreateEntityCommand<DataTransferObject>[] createCommands)
		{
			return
				SendCommands(() => createCommands.Select(command => new CreateCommand {Dto = command.Dto}).ToArray());
		}

		public ICallback Send(params IUpdateEntityCommand<DataTransferObject>[] updateCommands)
		{
			return
				SendCommands(
					() =>
					updateCommands.Select(
						x =>
						new UpdateCommand
							{Dto = x.Dto, ChangedFields = x.ChangedFields.Select(y => y.ToString()).ToArray()}).
						ToArray
						());
		}

		public ICallback Send(params IDeleteEntityCommand[] deleteCommands)
		{
			return
				SendCommands(
					() => deleteCommands.Select(x => new DeleteCommand {Id = x.ID, DtoType = new DtoType(x.DtoType)}).ToArray());
		}

		private ICallback SendCommands(Func<ITargetProcessMessage[]> getCommands)
		{
			SetOutHeaders();
			var commandsToSend = getCommands();
			return _bus.Send(commandsToSend);
		}

		#endregion

		public void SendLocal(params IPluginLocalMessage[] messages)
		{
			SetOutHeaders();
			_bus.SendLocal(messages);
		}

		public void SetOutSagaId(Guid id)
		{
			_bus.SetOutSagaId(id);
		}

		public void SendLocalWithContext(ProfileName profileName, AccountName accountName, IMessage message)
		{
			_bus.SetOut(profileName);
			_bus.SetOut(accountName);
			_bus.SendLocal(message);
		}

		public void SendLocalUiWithContext(ProfileName profileName, AccountName accountName, IMessage message)
		{
			_bus.SetOut(profileName);
			_bus.SetOut(accountName);
			_bus.SendLocalUi(message);
		}

		public void Send(params IPluginLifecycleMessage[] pluginLifecycleMessages)
		{
			SetOutHeaders();
			//Lifecycle Message is always high priority.
			_bus.SendToUi(pluginLifecycleMessages);
		}

		public void Reply(params ITargetProcessMessage[] messages)
		{
			//Reply messages are treated as high priority because TargetProcess is waiting.
			_bus.ReplyToUi(messages);
		}

		public ICallback Send(params ITargetProcessMessage[] messages)
		{
			SetOutHeaders();
			return _bus.Send(messages);
		}

		public void SendLocal<T>(Action<T> messageConstructor) where T : IPluginLocalMessage
		{
			SetOutHeaders();
			_bus.SendLocal(messageConstructor);
		}

		public ICallback Send<T>(Action<T> messageConstructor) where T : ITargetProcessMessage
		{
			SetOutHeaders();
			return _bus.Send(messageConstructor);
		}

		private void SetOutHeaders()
		{
			_bus.SetOut(_bus.GetInAccountName());
			_bus.SetOut(_bus.GetInProfileName());
		}

		public ICallback Send(string destination, params ITargetProcessMessage[] messages)
		{
			SetOutHeaders();
			return _bus.Send(destination, messages);
		}

		public ICallback Send<T>(string destination, Action<T> messageConstructor) where T : ITargetProcessMessage
		{
			SetOutHeaders();
			return _bus.Send(destination, messageConstructor);
		}

		public void Send(string destination, string correlationId, params ITargetProcessMessage[] messages)
		{
			SetOutHeaders();
			_bus.Send(destination, correlationId, messages);
		}

		public void Send<T>(string destination, string correlationId, Action<T> messageConstructor)
			where T : ITargetProcessMessage
		{
			SetOutHeaders();
			_bus.Send(destination, correlationId, messageConstructor);
		}

		public void HandleCurrentMessageLater()
		{
			SetOutHeaders();
			_bus.HandleCurrentMessageLater();
		}

		public void DoNotContinueDispatchingCurrentMessageToHandlers()
		{
			SetOutHeaders();
			_bus.DoNotContinueDispatchingCurrentMessageToHandlers();
		}
	}
}