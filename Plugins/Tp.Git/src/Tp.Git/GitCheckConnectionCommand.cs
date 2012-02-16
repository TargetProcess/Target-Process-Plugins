// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NGit.Transport;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;
using System.Linq;

namespace Tp.Git
{
	public class GitCheckConnectionCommand : VcsCheckConnectionCommand<GitPluginProfile>
	{
		private GitRepositoryFolder _folder;

		protected override void CheckStartRevision(GitPluginProfile settings, IVersionControlSystem versionControlSystem, PluginProfileErrorCollection errors)
		{
			settings.ValidateStartRevision(errors);
		}

		protected override void OnCheckConnection(PluginProfileErrorCollection errors, GitPluginProfile settings)
		{
			settings.ValidateUri(errors);

			if (!errors.Any())
			{
				_folder = GitRepositoryFolder.Create(settings.Uri);
				var nativeGit = NGit.Api.Git.Init().SetDirectory(_folder.Value).Call();
				var transport = Transport.Open(nativeGit.GetRepository(), settings.Uri);
				try
				{
					transport.SetCredentialsProvider(new UsernamePasswordCredentialsProvider(settings.Login, settings.Password));
					transport.OpenFetch();
				}
				catch
				{
					transport.Close();
					throw;
				}
			}
		}

		protected override void OnExecuted(GitPluginProfile profile)
		{
			base.OnExecuted(profile);

			if (_folder != null && _folder.Exists())
			{
				_folder.Delete();
			}
		}
	}
}