// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NServiceBus;
using StructureMap;

namespace Tp.MashupManager
{
	public class MashupsOnPluginStartSender : IWantCustomInitialization
	{
		public void Init()
		{
			var profile = ObjectFactory.GetInstance<ISingleProfile>().Profile;

			if (profile == null) return;

			var mashupNames = profile.GetProfile<MashupManagerProfile>().MashupNames;
			var bus = ObjectFactory.GetInstance<IBus>();

			foreach (var mashupName in mashupNames)
			{
				var mashup = profile.Get<MashupDto>(mashupName).SingleOrDefault();

				if (mashup != null)
				{
					bus.Send(mashup.CreatePluginMashupMessage());
				}
			}
		}
	}
}