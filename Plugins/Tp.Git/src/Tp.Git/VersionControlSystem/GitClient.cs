// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NGit;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using NGit.Transport;
using StructureMap;
using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
	public class GitClient
	{
		private readonly NGit.Api.Git _git;
		private readonly ISourceControlConnectionSettingsSource _settings;

		public GitClient(ISourceControlConnectionSettingsSource settings)
		{
			_settings = settings;
			_git = GetClient(settings);
		}

		public IEnumerable<RevisionRange> GetFromTillHead(DateTime from, int pageSize)
		{
			Fetch();

			var revWalk = CreateRevWalker();

			try
			{
				var filter = ApplyNoMergesFilter(CommitTimeRevFilter.After(from));
				revWalk.SetRevFilter(filter);

				var commits =
					(from revision in revWalk orderby revision.GetCommitTime() ascending select revision).ToArray().Split(pageSize);

				var fromTillHead =
					commits.Select(x => new RevisionRange(x.First().ConvertToRevisionId(), x.Last().ConvertToRevisionId())).ToArray();
				return fromTillHead;
			}
			finally
			{
				revWalk.Dispose();
			}
		}

		private static RevFilter ApplyNoMergesFilter(RevFilter filter)
		{
			return AndRevFilter.Create(new[] {RevFilter.NO_MERGES, filter});
		}

		public IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize)
		{
			var addSeconds = revisionId.Time.Value.AddSeconds(1);
			return GetFromTillHead(addSeconds, pageSize);
		}

		private RevWalk CreateRevWalker()
		{
			var repository = _git.GetRepository();
			try
			{
				var revWalk = new RevWalk(repository);

				foreach (var reference in repository.GetAllRefs())
				{
					revWalk.MarkStart(revWalk.ParseCommit(reference.Value.GetObjectId()));
				}
				return revWalk;
			}
			finally
			{
				repository.Close();
			}
		}

		public string[] RetrieveAuthors(DateRange dateRange)
		{
			var revWalk = CreateRevWalker();
			revWalk.SetRevFilter(CommitTimeRevFilter.Between(dateRange.StartDate.GetValueOrDefault(),
			                                                 dateRange.EndDate.GetValueOrDefault()));

			var commits = revWalk.ToArray();

			return (from revision in commits select revision.GetAuthorIdent().GetName()).Distinct().ToArray();
		}

		public RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset)
		{
			var revWalk = CreateRevWalker();
			try
			{
				RevFilter betweenFilter = CommitTimeRevFilter.Between(((GitRevisionId) fromChangeset).Time,
				                                                      ((GitRevisionId) toChangeset).Time);

				revWalk.SetRevFilter(ApplyNoMergesFilter(betweenFilter));
				var commits = revWalk.ToArray();
				return commits.Select(commit => commit.ConvertToRevisionInfo(_git.GetRepository())).ToArray();
			}
			finally
			{
				revWalk.Dispose();
			}
		}

		public RevCommit GetCommit(RevisionId id)
		{
			var revWalk = CreateRevWalker();
			try
			{
				return revWalk.ParseCommit(ObjectId.FromString(id.Value));
			}
			finally
			{
				revWalk.Dispose();
			}
		}

		~GitClient()
		{
			BatchingProgressMonitor.ShutdownNow();
		}

		public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize)
		{
			Fetch();

			var revWalk = CreateRevWalker();
			try
			{
				var filter = CommitTimeRevFilter.Between(((GitRevisionId) from).Time.AddSeconds(1),
				                                         ((GitRevisionId) to).Time.AddSeconds(-1));
				revWalk.SetRevFilter(ApplyNoMergesFilter(filter));

				var commits =
					(from revision in revWalk orderby revision.GetCommitTime() ascending select revision).ToArray().Split(pageSize);
				var fromTillHead =
					commits.Select(x => new RevisionRange(x.First().ConvertToRevisionId(), x.Last().ConvertToRevisionId())).ToArray();
				return fromTillHead;
			}
			finally
			{
				revWalk.Dispose();
			}
		}

		private static NGit.Api.Git GetClient(ISourceControlConnectionSettingsSource settings)
		{
			var repositoryFolder = GetLocalRepository(settings);
			if (IsRepositoryUriChanged(repositoryFolder, settings))
			{
				repositoryFolder.Delete();
				repositoryFolder = GitRepositoryFolder.Create(settings.Uri);
				var repoFolderStorage = Repository.Get<GitRepositoryFolder>();
				repoFolderStorage.ReplaceWith(repositoryFolder);
			}

			NGit.Api.Git nativeGit;

			try
			{
				var credentialsProvider = new UsernamePasswordCredentialsProvider(settings.Login, settings.Password);
				if (repositoryFolder.Exists())
				{
					nativeGit = NGit.Api.Git.Open(repositoryFolder.Value);
				}
				else
				{
					nativeGit = NGit.Api.Git.CloneRepository()
						.SetURI(settings.Uri)
						.SetNoCheckout(true)
						.SetCredentialsProvider(credentialsProvider)
						.SetDirectory(repositoryFolder.Value).Call();
				}
			}
			catch (ArgumentNullException exception)
			{
				throw new ArgumentException(
					GitCheckConnectionErrorResolver.INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE, exception);
			}

			return nativeGit;
		}

		private void Fetch()
		{
			var credentialsProvider = new UsernamePasswordCredentialsProvider(_settings.Login, _settings.Password);

			_git.Clean().Call();
			_git.Fetch().SetCredentialsProvider(credentialsProvider).SetRemoveDeletedRefs(true).Call();
		}

		private static bool IsRepositoryUriChanged(GitRepositoryFolder repositoryFolder,
		                                           ISourceControlConnectionSettingsSource settings)
		{
			return (settings.Uri.ToLower() != repositoryFolder.RepoUri.ToLower()) && repositoryFolder.Exists();
		}

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

		public string GetFileContent(RevCommit commit, string path)
		{
			return commit.GetFileContent(path, _git.GetRepository());
		}
	}
}