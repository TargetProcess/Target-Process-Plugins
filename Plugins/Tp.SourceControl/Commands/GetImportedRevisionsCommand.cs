// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.SourceControl.RevisionStorage;

namespace Tp.SourceControl.Commands
{
	public class GetImportedRevisionsCommand : IPluginCommand
	{
		public PluginCommandResponseMessage Execute(string args)
		{
			var bugIds = args.Deserialize<int[]>();

			var repository = ObjectFactory.GetInstance<IRevisionStorageRepository>();
			var revisions = repository.GetImportedTpIds(bugIds);

			return new PluginCommandResponseMessage
			       	{PluginCommandStatus = PluginCommandStatus.Succeed, ResponseData = revisions.Serialize()};
		}

		public string Name
		{
			get { return "GetImportedRevisions"; }
		}
	}
}