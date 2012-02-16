// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.MashupManager.MashupStorage;

namespace Tp.MashupManager
{
	public class MashupsOnTargetProcessStartSender : IHandleMessages<TargetProcessStartedMessage>
	{
		private IEnumerable<IAccountReadonly> Accounts
		{
			get { return ObjectFactory.GetInstance<IAccountCollection>(); }
		}

		private IMashupScriptStorage MashupStorage
		{
			get { return ObjectFactory.GetInstance<IMashupScriptStorage>(); }
		}

		public void Handle(TargetProcessStartedMessage message)
		{
			foreach (var account in Accounts)
			{
				if (!account.Profiles.Any())
				{
					continue;
				}

				var profile = account.Profiles.First();
				var mashupNames = profile.GetProfile<MashupManagerProfile>().MashupNames;
				var bus = ObjectFactory.GetInstance<IBus>();

				foreach (var mashupName in mashupNames)
				{
					var mashup = MashupStorage.GetMashup(account.Name, mashupName);

					if (mashup != null)
					{
						bus.Send(mashup.CreatePluginMashupMessage(account.Name));
					}
				}
			}
		}
	}
}