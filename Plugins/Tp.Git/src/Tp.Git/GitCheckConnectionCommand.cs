// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using StructureMap;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git
{
	public class GitCheckConnectionCommand : VcsCheckConnectionCommand<GitPluginProfile>
	{
		private GitRepositoryFolder _folder;

		protected override void CheckStartRevision(GitPluginProfile settings, IVersionControlSystem versionControlSystem, PluginProfileErrorCollection errors)
		{
			settings.ValidateStartRevision(errors);
		}

		protected override IVersionControlSystem CreateVcs(GitPluginProfile settings)
		{
			_folder = GitRepositoryFolder.Create(settings.Uri);
			return new GitVersionControlSystem(settings, _folder, ObjectFactory.GetInstance<ICheckConnectionErrorResolver>(),
			                                   ObjectFactory.GetInstance<IActivityLogger>(), new CheckConnectionProgressMonitor());
		}

		protected override void OnExecuted(GitPluginProfile profile)
		{
			base.OnExecuted(profile);

			_folder.Delete();
		}
	}
}