// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Core;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	public class SyncNowCommand : IPluginCommand
	{
		private readonly ILocalBus _localBus;
		private readonly IPluginContext _pluginContext;
		private readonly IProfileCollection _profileCollection;

		public SyncNowCommand(ILocalBus localBus, IPluginContext pluginContext, IProfileCollection profileCollection)
		{
			_localBus = localBus;
			_pluginContext = pluginContext;
			_profileCollection = profileCollection;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			if (_pluginContext.ProfileName.IsEmpty)
			{
				throw new ApplicationException("'SyncNow' command should be executed for concrete profile only.");
			}

			if (_profileCollection[_pluginContext.ProfileName].IsNull)
			{
				throw new ApplicationException(string.Format("No profile with name '{0}' was found.",
				                                             _pluginContext.ProfileName.Value));
			}

			var lastSyncDates = _profileCollection[_pluginContext.ProfileName].Get<LastSyncDate>();
			_localBus.SendLocal(new TickMessage(lastSyncDates.Select(d => d.Value).FirstOrDefault()));

			lastSyncDates.ReplaceWith(new LastSyncDate(CurrentDate.Value));

			return new PluginCommandResponseMessage {PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		public string Name
		{
			get { return EmbeddedPluginCommands.SyncNow; }
		}
	}
}