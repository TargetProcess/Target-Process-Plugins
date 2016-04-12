// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;

namespace Tp.Bugzilla.CustomCommand
{
	public class GetBugzillaBugsCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args, UserDTO user)
		{
			var bugIds = args.Deserialize<int[]>();

			var repository = ObjectFactory.GetInstance<IBugzillaInfoStorageRepository>();
			var bugs = repository.GetAllBugzillaBugs(bugIds);

			return new PluginCommandResponseMessage
			       	{PluginCommandStatus = PluginCommandStatus.Succeed, ResponseData = bugs.Serialize()};
		}

		public string Name
		{
			get { return "GetBugzillaBugs"; }
		}
	}
}