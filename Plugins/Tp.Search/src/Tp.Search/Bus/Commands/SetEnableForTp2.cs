// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using System.Linq;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Search.Bus.Commands
{
	public class SetEnableForTp2 : IPluginCommand
	{
		public const string CommandName = "SetEnableForTp2";
		private readonly IPluginContext _pluginContext;

		private readonly IActivityLogger _log;

		private readonly IProfileCollection _profileCollection;

		private readonly ITpBus _bus;

		public SetEnableForTp2(IPluginContext pluginContext, IActivityLogger log, IProfileCollection profileCollection, ITpBus bus)
		{
			_pluginContext = pluginContext;
			_log = log;
			_profileCollection = profileCollection;
			_bus = bus;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			bool enable = false;
			if (args != null && !bool.TryParse(args, out enable)) // null means false
			{
				{
					return new PluginCommandResponseMessage
					{
						PluginCommandStatus = PluginCommandStatus.Error,
						ResponseData = "Invalid Value"
					};
				}
			}

			if (this._profileCollection.All(x => x.Name != this._pluginContext.ProfileName))
			{
				return new PluginCommandResponseMessage
				{
					PluginCommandStatus = PluginCommandStatus.Fail,
					ResponseData =
						"Profile {0} is not found".Fmt(this._pluginContext.ProfileName.Value)
				};
			}

			var profile = this._profileCollection[this._pluginContext.ProfileName];
			var settings = profile.Settings as SearcherProfile;
			if (settings == null)
			{
				settings = new SearcherProfile();
				profile.Settings = settings;
			}
			settings.EnabledForTp2 = enable;
			profile.Save();
			var message = "New search is " + (enable ? "enabled" : "disabled") + " for TP2";
			this._log.Info(message);
			this._bus.SendLocal(new IPluginLocalMessage[] { new SendEnableForTp2Mashup() });
			return new PluginCommandResponseMessage
			{
				PluginCommandStatus = PluginCommandStatus.Succeed,
				ResponseData = message.Serialize()
			};
		}

		public string Name
		{
			get { return CommandName; }
		}
	}
}