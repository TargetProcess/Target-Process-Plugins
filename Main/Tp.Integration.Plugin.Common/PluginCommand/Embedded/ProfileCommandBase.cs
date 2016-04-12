// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginLifecycle;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
	public abstract class ProfileCommandBase
	{
		private readonly IProfileCollection _profileCollection;
		private readonly ITpBus _bus;
		private readonly IPluginContext _pluginContext;

		protected ProfileCommandBase(IProfileCollection profileCollection, ITpBus bus, IPluginContext pluginContext)
		{
			_profileCollection = profileCollection;
			_bus = bus;
			_pluginContext = pluginContext;
		}

		protected IPluginContext PluginContext
		{
			get { return _pluginContext; }
		}

		protected IProfileCollection ProfileCollection
		{
			get { return _profileCollection; }
		}

		protected void ChangeProfiles(Action<IProfileCollection> changeProfilesAction)
		{
			changeProfilesAction(_profileCollection);

			var message = CreatePluginInfoChangedMessage(PluginContext.AccountName,
			                                             _profileCollection.Select(profile => profile.ConvertToPluginProfile()).
			                                             	ToArray());
			_bus.Send(message);
		}

		private PluginAccountMessageSerialized CreatePluginInfoChangedMessage(AccountName accountName,
		                                                            params PluginProfile[] profiles)
		{
			var pluginAccount = PluginInfoSender.CreatePluginAccount(PluginContext.PluginName, accountName, profiles);
			return new PluginAccountMessageSerialized {SerializedMessage = new[]{pluginAccount}.Serialize()};
		}

		protected void SendProfileChangedLocalMessage(ProfileName profileName, ITargetProcessMessage profileMessage)
		{
			_bus.SendLocalWithContext(profileName, PluginContext.AccountName, profileMessage);
		}
	}
}
