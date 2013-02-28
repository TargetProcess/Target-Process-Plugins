// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus.Utils;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Messages.ServiceBus.UnicastBus;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using log4net;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	internal class DeleteUnusedQueuesCommand : IPluginCommand
	{
		private readonly IAccountCollection _accountCollection;
		private readonly IMsmqTransport _msmqTransport;
		public static string Deleteunusedqueues = "DeleteUnusedQueues";

		public DeleteUnusedQueuesCommand(IAccountCollection accountCollection, IMsmqTransport msmqTransport)
		{
			_accountCollection = accountCollection;
			_msmqTransport = msmqTransport;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			foreach (var account in _accountCollection)
			{
				if (!account.Profiles.Any())
				{
					_msmqTransport.TryDeleteQueue(account.Name.Value);
				}

				_msmqTransport.TryDeleteUiQueue(account.Name.Value);
			}

			return new PluginCommandResponseMessage { ResponseData = string.Empty, PluginCommandStatus = PluginCommandStatus.Succeed };
		}

		public string Name
		{
			get { return Deleteunusedqueues; }
		}
	}
}