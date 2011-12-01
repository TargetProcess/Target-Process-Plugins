// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using System;
using System.Linq;
using NGit;
using StructureMap;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
	public class GitVersionControlSystem : SourceControl.VersionControlSystem.VersionControlSystem
	{
		public GitVersionControlSystem(ISourceControlConnectionSettingsSource settings,
		                               ICheckConnectionErrorResolver errorResolver, IActivityLogger logger)
			: this(settings, GetLocalRepository(settings), errorResolver, logger, NullProgressMonitor.INSTANCE)
		{
		}

		public GitVersionControlSystem(ISourceControlConnectionSettingsSource settings, GitRepositoryFolder folder,
		                               ICheckConnectionErrorResolver errorResolver, IActivityLogger logger,
		                               ProgressMonitor progressMonitor)
			: base(settings, errorResolver, logger)
		{
			_git = GitClient.Connect(folder, _settings, progressMonitor);
		}

		private readonly GitClient _git;

		private static IStorageRepository Repository
		{
			get { return ObjectFactory.GetInstance<IStorageRepository>(); }
		}

		private static GitRepositoryFolder GetLocalRepository(ISourceControlConnectionSettingsSource settings)
		{
			var repoFolderStorage = Repository.Get<GitRepositoryFolder>();
			if (repoFolderStorage.Empty())
			{
				var repositoryFolder = GitRepositoryFolder.Create(settings.Uri);
				repoFolderStorage.ReplaceWith(repositoryFolder);
				return repositoryFolder;
			}

			return repoFolderStorage.Single();
		}

		public override RevisionInfo[] GetRevisions(RevisionRange revisionRange)
		{
			return _git.GetRevisions(revisionRange.FromChangeset, revisionRange.ToChangeset);
		}

		public override string GetTextFileContent(RevisionId changeset, string path)
		{
			throw new NotImplementedException();
		}

		public override byte[] GetBinaryFileContent(RevisionId changeset, string path)
		{
			throw new NotImplementedException();
		}

		public override void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors)
		{
			GitRevisionId revisionId = revision;
			if (revisionId.Value > GitRevisionId.UtcTimeMax)
			{
				_errorResolver.HandleConnectionError(new InvalidRevisionException(), errors);
			}
		}

		public override string[] RetrieveAuthors(DateRange dateRange)
		{
			GitRevisionId from = dateRange.StartDate.GetValueOrDefault();
			GitRevisionId to = dateRange.EndDate.GetValueOrDefault();

			return _git.RetrieveAuthors(from, to);
		}

		public override RevisionRange[] GetFromTillHead(RevisionId @from, int pageSize)
		{
			return _git.GetFromTillHead(from, pageSize).ToArray();
		}

		public override RevisionRange[] GetAfterTillHead(RevisionId @from, int pageSize)
		{
			return _git.GetAfterTillHead(from, pageSize).ToArray();
		}

		public override RevisionRange[] GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize)
		{
			return _git.GetFromAndBefore(from, to, pageSize).ToArray();
		}
	}
}