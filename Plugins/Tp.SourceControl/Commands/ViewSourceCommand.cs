// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.SourceControl.RevisionStorage;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Commands
{
	public class ViewSourceCommand : IPluginCommand
	{
		private readonly IRevisionStorageRepository _repository;
		private readonly IVersionControlSystemFactory _vcsFactory;

		public ViewSourceCommand(IRevisionStorageRepository repository, IVersionControlSystemFactory vcsFactory)
		{
			_repository = repository;
			_vcsFactory = vcsFactory;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			var response = new ContentResponse();
			var fileArgs = args.Deserialize<FileViewDiffArgs>();

			try
			{
				var revision = _repository.GetRevisionId(fileArgs.TpRevisionId);

				if (revision != null)
				{
					var vcs = _vcsFactory.Get(revision.ConnectionSettings);
					response.Content = vcs.GetTextFileContent(revision.RevisionId.RevisionId, fileArgs.Path).Replace("\t", "    ");
				}

				return new PluginCommandResponseMessage
				       	{
				       		PluginCommandStatus = PluginCommandStatus.Succeed,
				       		ResponseData = response.Serialize()
				       	};
			}
			catch
			{
				return new PluginCommandResponseMessage
				       	{
				       		PluginCommandStatus = PluginCommandStatus.Error,
				       		ResponseData = "Unable to connect to a remote repository"
				       	};
			}
		}

		public string Name
		{
			get { return "ViewSource"; }
		}
	}
}