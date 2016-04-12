// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Gateways
{
	public class PluginGateway : IHandleMessages<ITargetProcessMessage>
	{
		private readonly ITpBus _bus;
		private readonly IPluginContext _pluginContext;
		private readonly IProfileCollection _profileCollection;

		public PluginGateway(ITpBus bus, IPluginContext pluginContext, IProfileCollection profileCollection)
		{
			_bus = bus;
			_pluginContext = pluginContext;
			_profileCollection = profileCollection;
		}

		public void Handle(ITargetProcessMessage message)
		{
			if (_pluginContext.ProfileName.IsEmpty)
			{
				if (!_profileCollection.Any())
				{
					var singleProfileProvider = ObjectFactory.TryGetInstance<ITargetProcessMessageWhenNoProfilesHandler>();
					if (singleProfileProvider != null)
					{
						singleProfileProvider.Handle(message);
					}
				}

				DispatchToProfiles(message, (x, y) => x.Send(y));
			}
		}

		private void DispatchToProfiles(ITargetProcessMessage message,
		                                Action<IProfileGateway, ITargetProcessMessage> sendAction)
		{
			_bus.DoNotContinueDispatchingCurrentMessageToHandlers();

			foreach (
				var gateway in _profileCollection.Select(profile => new ProfileGateway(profile, _pluginContext.AccountName, _bus)))
			{
				sendAction(gateway, message);
			}
		}
	}
}