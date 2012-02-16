// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.SourceControl.Diff;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Commands
{
	public class ViewDiffCommand : IPluginCommand
	{
		private readonly IRevisionStorageRepository _repository;
		private readonly IVersionControlSystemFactory _vcsFactory;

		public ViewDiffCommand(IRevisionStorageRepository repository, IVersionControlSystemFactory vcsFactory)
		{
			_repository = repository;
			_vcsFactory = vcsFactory;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			var fileArgs = args.Deserialize<FileViewDiffArgs>();

			try
			{
				var revision = _repository.GetRevisionId(fileArgs.TpRevisionId);
				var diff = new DiffResult();

				if (revision != null)
				{
					var vcs = _vcsFactory.Get(revision.ConnectionSettings);
					diff = vcs.GetDiff(revision.RevisionId.RevisionId, fileArgs.Path);
				}

				return new PluginCommandResponseMessage
				       	{
				       		PluginCommandStatus = PluginCommandStatus.Succeed,
				       		ResponseData = diff.Serialize()
				       	};
			}
			catch
			{
				return new PluginCommandResponseMessage
				       	{
				       		PluginCommandStatus = PluginCommandStatus.Error,
				       		ResponseData = "Unable to connect to a remote repository."
				       	};
			}
		}

		public string Name
		{
			get { return "ViewDiff"; }
		}
	}
}