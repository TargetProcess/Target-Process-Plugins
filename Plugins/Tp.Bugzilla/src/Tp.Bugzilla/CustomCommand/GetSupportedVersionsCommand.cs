// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;

namespace Tp.Bugzilla.CustomCommand
{
	public class GetSupportedVersionsCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args)
		{
			return new PluginCommandResponseMessage
			       	{
			       		PluginCommandStatus = PluginCommandStatus.Succeed,
			       		ResponseData = BugzillaInfo.SupportedVersions.OrderBy(x => x).ToArray().Serialize()
			       	};
		}

		public string Name
		{
			get { return "GetSupportedVersions"; }
		}
	}
}