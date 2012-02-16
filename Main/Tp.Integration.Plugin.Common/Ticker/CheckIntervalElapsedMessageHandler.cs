// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Core;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;

namespace Tp.Integration.Plugin.Common.Ticker
{
	public class CheckIntervalElapsedMessageHandler : IHandleMessages<CheckIntervalElapsedMessage>
	{
		private readonly IBus _bus;
		private readonly IPluginMetadata _pluginMetadata;
		private readonly IAccountCollection _collection;

		public CheckIntervalElapsedMessageHandler(IBus bus, IPluginMetadata pluginMetadata,
		                                          IAccountCollection collection)
		{
			_bus = bus;
			_pluginMetadata = pluginMetadata;
			_collection = collection;
		}

		public void Handle(CheckIntervalElapsedMessage message)
		{
			var accountProfiles = _collection.SelectMany(acc => acc.Profiles, (acc, profile) => new
			                                                                              	{
			                                                                              		Account = acc, 
																								Profile = profile
			                                                                              	});
			foreach (var accountProfile in accountProfiles)
			{
				var lastSyncDate = accountProfile.Profile.Get<LastSyncDate>().FirstOrDefault();
				if (IsTimeToSyncronize(accountProfile.Profile, lastSyncDate) && accountProfile.Profile.Initialized)
				{
					var profile = accountProfile.Profile;
					_bus.SetOut(profile.ConvertToPluginProfile().Name);
					_bus.SetOut(accountProfile.Account.Name);
					
					_bus.SendLocal(lastSyncDate != null
					               	? new TickMessage(lastSyncDate.Value)
					               	: new TickMessage(null));

					ObjectFactory.GetInstance<ILogManager>().GetLogger(GetType()).Info("TickMesage sent");

					profile.Get<LastSyncDate>().ReplaceWith(new LastSyncDate(CurrentDate.Value));
				}
			}
		}

		private bool IsTimeToSyncronize(IProfileReadonly profile, LastSyncDate lastSyncDate)
		{
			if (!_pluginMetadata.IsSyncronizableProfile)
				return false;
			var syncronizableProfile = profile.Settings as ISynchronizableProfile;

			return IsFirstSynchronization(lastSyncDate) || CurrentDate.Value.Subtract(lastSyncDate.Value).TotalMinutes >=
			       syncronizableProfile.SynchronizationInterval;
		}

		private static bool IsFirstSynchronization(LastSyncDate lastSyncDate)
		{
			return lastSyncDate == null;
		}
	}
}