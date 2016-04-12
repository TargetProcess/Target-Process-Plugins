// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;
using StructureMap;
using Tp.Bugzilla.LegacyProfileConversion;
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Logging;

namespace Tp.Bugzilla.CustomCommand
{
	public class ConvertProfileCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args, UserDTO user)
		{
			var logger = ObjectFactory.GetInstance<ILogManager>();
			var log = logger.GetLogger(GetType());

			log.InfoFormat("Bugzilla Legacy Profile '{0}' converting started", args);

			var profile = args.Deserialize<ProfileNameHolder>();
			ObjectFactory.GetInstance<ILocalBus>().SendLocal(new ConvertLegacyProfileLocalMessage{LegacyProfileName = profile.ProfileName});
			return new PluginCommandResponseMessage {PluginCommandStatus = PluginCommandStatus.Succeed};
		}

		public string Name
		{
			get { return "ConvertProfile"; }
		}
	}

	[DataContract]
	public class ProfileNameHolder
	{
		[DataMember]
		public string ProfileName { get; set; }
	}
}