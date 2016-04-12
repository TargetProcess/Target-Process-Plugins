// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common;
using StructureMap;

namespace Tp.Search.Bus
{
	public class MashupForTp2
		: IHandleMessages<SendEnableForTp2Mashup>, IWantToRunAtStartup
	{
		private readonly ITpBus _bus;

		private static string[] GetEnabledAccounts()
		{
			var accountCollection = ObjectFactory.GetInstance<IAccountCollection>();
			var accounts = accountCollection
				.Where(account => account.Profiles.Any(profile => profile.Name == SearcherProfile.Name && profile.Settings is SearcherProfile && (profile.Settings as SearcherProfile).EnabledForTp2))
				.Select(x => x.Name.Value)
				.ToArray();

			return accounts.Length == 0
				? new[] { "@@@no-account@@@" }
				: accounts;
		}

		public MashupForTp2(ITpBus bus)
		{
			_bus = bus;
		}

		private static IEnumerable<PluginMashupScript> GetMashupScripts()
		{
			yield return new PluginMashupScript { FileName = "Enable.js", ScriptContent = Resources.EnableInTp2 };

			var accountsString = "Accounts:" + string.Join(",", GetEnabledAccounts());
			yield return new PluginMashupScript { FileName = "Enable.cfg", ScriptContent = accountsString };
		}

		private void SendMashupMessage()
		{
			var pluginMessage = new PluginMashupMessage
			{
				PluginMashupScripts = GetMashupScripts().ToArray(),
				PluginName = "Search",
				Placeholders = new[] { "tp2placeholder" },
				MashupName = "SearchTP2Enabler",
				AccountName = AccountName.Empty
			};

			_bus.Send(pluginMessage);
		}

		public void Run()
		{
			SendMashupMessage();
		}

		public void Stop()
		{
		}

		public void Handle(SendEnableForTp2Mashup message)
		{
			SendMashupMessage();
		}
	}
}