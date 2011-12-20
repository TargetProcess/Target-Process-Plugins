// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NUnit.Framework;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.TaskCreator.Tests
{
	public class TpBusMock : ITpBus
	{
		private readonly List<ICreateEntityCommand<DataTransferObject>> _createCommands =
			new List<ICreateEntityCommand<DataTransferObject>>();

		private readonly List<ITargetProcessMessage> _sentCommands = new List<ITargetProcessMessage>();

		private readonly List<IUpdateEntityCommand<DataTransferObject>> _updateCommands =
			new List<IUpdateEntityCommand<DataTransferObject>>();

		private readonly List<IDeleteEntityCommand> _deleteCommands = new List<IDeleteEntityCommand>();
		private readonly List<IPluginLocalMessage> _localMessages = new List<IPluginLocalMessage>();

		public event EventHandler<CommandEventArgs> MessageSent;
		public event EventHandler<CommandEventArgs> LocalMessageSent;

		public List<ICreateEntityCommand<DataTransferObject>> CreateCommands
		{
			get { return _createCommands; }
		}

		public List<IUpdateEntityCommand<DataTransferObject>> UpdateCommands
		{
			get { return _updateCommands; }
		}

		public List<IDeleteEntityCommand> DeleteCommands
		{
			get { return _deleteCommands; }
		}

		public bool IsEmpty
		{
			get { return CreateCommands.Count == 0 && UpdateCommands.Count == 0 && DeleteCommands.Count == 0; }
		}

		public List<IPluginLocalMessage> LocalMessages
		{
			get { return _localMessages; }
		}

		public List<ITargetProcessMessage> SentCommands
		{
			get { return _sentCommands; }
		}

		public void SendLocal<T>(Action<T> messageConstructor) where T : IPluginLocalMessage
		{
		}

		public ICallback Send(params ITargetProcessMessage[] messages)
		{
			_sentCommands.AddRange(messages);
			MessageSent.Raise(this, new CommandEventArgs(messages));
			return null;
		}

		public ICallback Send<T>(Action<T> messageConstructor) where T : ITargetProcessMessage
		{
			return null;
		}

		public ICallback Send(string destination, params ITargetProcessMessage[] messages)
		{
			return null;
		}

		public ICallback Send<T>(string destination, Action<T> messageConstructor) where T : ITargetProcessMessage
		{
			return null;
		}

		public void Send(string destination, string correlationId, params ITargetProcessMessage[] messages)
		{
		}

		public void Send<T>(string destination, string correlationId, Action<T> messageConstructor)
			where T : ITargetProcessMessage
		{
		}

		public void HandleCurrentMessageLater()
		{
		}

		public void DoNotContinueDispatchingCurrentMessageToHandlers()
		{
		}

		public void SetOutSagaId(Guid id)
		{
		}

		public void SendLocalWithContext(ProfileName profileName, AccountName accountName, ITargetProcessMessage message)
		{
		}

		public void Send(params IPluginLifecycleMessage[] pluginLifecycleMessages)
		{
			throw new NotSupportedException();
		}

		public void Reply(params ITargetProcessMessage[] messages)
		{
			throw new NotImplementedException();
		}

		public void SendLocalUiWithContext(ProfileName profileName, AccountName accountName, ITargetProcessMessage message)
		{
		}

		public ICallback Send(params ICreateEntityCommand<DataTransferObject>[] createCommands)
		{
			CreateCommands.AddRange(createCommands);
			MessageSent.Raise(this, new CommandEventArgs(createCommands));
			return null;
		}

		public ICallback Send(params IUpdateEntityCommand<DataTransferObject>[] updateCommands)
		{
			UpdateCommands.AddRange(updateCommands);
			MessageSent.Raise(this, new CommandEventArgs(updateCommands));
			return null;
		}

		public ICallback Send(params IDeleteEntityCommand[] deleteCommands)
		{
			DeleteCommands.AddRange(deleteCommands);
			MessageSent.Raise(this, new CommandEventArgs(deleteCommands));
			return null;
		}

		public void SendLocal(params IPluginLocalMessage[] messages)
		{
			LocalMessages.AddRange(messages);
			LocalMessageSent.Raise(this, new CommandEventArgs(messages));
		}
	}

	public class CommandEventArgs : EventArgs
	{
		public object[] Messages { get; set; }

		public CommandEventArgs(object[] messages)
		{
			Messages = messages;
		}
	}

	public static class TpBusMockExtensions
	{
		public static void CreateCommandShouldMatch<T>(this TpBusMock commandBusMock, params Predicate<T>[] predicates)
			where T : class
		{
			commandBusMock.CreateCommands.Count.Should(Is.EqualTo(1),
			                                           "1 create command should be sent");
			commandBusMock.CreateCommands[0].Should(Is.InstanceOf(typeof (T)));
			predicates.ToList().ForEach(x => x(commandBusMock.CreateCommands[0] as T));
		}

		public static void UpdateCommandShouldMatch<T>(this TpBusMock commandBusMock, params Predicate<T>[] predicates)
			where T : class
		{
			commandBusMock.UpdateCommands.Count.Should(Is.EqualTo(1),
			                                           "1 create command should be sent");
			commandBusMock.UpdateCommands[0].Should(Is.InstanceOf(typeof (T)));
			predicates.ToList().ForEach(x => x(commandBusMock.UpdateCommands[0] as T));
		}

		public static void SentCommandsShouldMatch<T>(this TpBusMock commandBusMock, params Predicate<T>[] predicates)
			where T : class
		{
			commandBusMock.SentCommands.Count.Should(Is.EqualTo(1),
			                                         "1 message should be sent");
			commandBusMock.SentCommands[0].Should(Is.InstanceOf(typeof (T)));
			predicates.ToList().ForEach(x => x(commandBusMock.SentCommands[0] as T));
		}

		public static void LocalMessageShouldMatch<T>(this TpBusMock localBusMock, params Predicate<T>[] predicates)
			where T : class
		{
			localBusMock.LocalMessages.Count.Should(Is.EqualTo(1),
			                                        "1 local message should be sent");
			localBusMock.LocalMessages[0].Should(Is.InstanceOf(typeof (T)));
			predicates.ToList().ForEach(x => x(localBusMock.LocalMessages[0] as T));
		}
	}
}