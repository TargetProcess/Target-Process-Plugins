// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Search.Bus
{
	public class SearcherOnTargetProcessStartSender : IHandleMessages<PluginInfoMessage>
	{
		private IEnumerable<IAccountReadonly> Accounts
		{
			get { return ObjectFactory.GetInstance<IAccountCollection>(); }
		}

		public void Handle(PluginInfoMessage message)
		{
			foreach (var account in Accounts)
			{
				if (!account.Profiles.Any())
				{
					continue;
				}

				var profile = account.Profiles.First();
				var searcherProfile = profile.GetProfile<SearcherProfile>();
			}
		}
	}
}