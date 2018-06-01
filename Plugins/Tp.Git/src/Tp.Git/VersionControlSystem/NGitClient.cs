// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NGit.Revwalk;
using NGit.Revwalk.Filter;
using NGit.Transport;
using Sharpen;
using Tp.Core;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;
using ObjectId = NGit.ObjectId;

namespace Tp.Git.VersionControlSystem
{
    public class NGitClientFactory : IGitClientFactory
    {
        public IGitClient Create(IGitConnectionSettings settings, IStorage<GitRepositoryFolder> folderStorage)
        {
            if (settings.UseSsh)
            {
                throw new NotSupportedException("This profile uses ssh connection, it's not supported in NGit");
            }
            return new NGitClient(settings, folderStorage);
        }
    }

    public class NGitClient: IGitClient
    {
        private const string ConnectionTimeoutInSecondsSectionName = "ConnectionTimeoutInSeconds";
        private readonly NGit.Api.Git _git;
        private readonly ISourceControlConnectionSettingsSource _settings;
        private readonly IStorage<GitRepositoryFolder> _folder;

        public NGitClient(ISourceControlConnectionSettingsSource settings, IStorage<GitRepositoryFolder> folder)
        {
            _settings = settings;
            _folder = folder;
            _git = GetClient(_settings);
        }

        public IEnumerable<RevisionRange> GetFromTillHead(DateTime from, int pageSize)
        {
            var revWalk = CreateRevWalker();

            try
            {
                var filter = ApplyNoMergesFilter(CommitTimeRevFilter.After(from));

                revWalk.SetRevFilter(filter);

                var commits = (from revision in revWalk orderby revision.GetCommitTime() ascending select revision).ToArray()
                    .Split(pageSize);
                var fromTillHead =
                    commits.Select(x => new RevisionRange(x.First().ConvertToRevisionId(), x.Last().ConvertToRevisionId())).ToArray();

                return fromTillHead;
            }
            finally
            {
                revWalk.Dispose();
            }
        }

        public IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize)
        {
            var addSeconds = revisionId.Time.Value.AddSeconds(1);
            return GetFromTillHead(addSeconds, pageSize);
        }

        private static RevFilter ApplyNoMergesFilter(RevFilter filter)
        {
            return AndRevFilter.Create(new[] { RevFilter.NO_MERGES, filter });
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

        private RevCommit GetCommit(RevisionId id)
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

        public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize)
        {
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

        private NGit.Api.Git GetClient(ISourceControlConnectionSettingsSource settings)
        {
            var repositoryFolder = GetLocalRepository(settings);
            if (IsRepositoryUriChanged(repositoryFolder, settings))
            {
                repositoryFolder.Delete();
                repositoryFolder = GitRepositoryFolder.Create(settings.Uri);
                _folder.ReplaceWith(repositoryFolder);
            }

            NGit.Api.Git nativeGit;

            try
            {
                var credentialsProvider = new UsernamePasswordCredentialsProvider(settings.Login, settings.Password);
                if (repositoryFolder.Exists())
                {
                    string path = repositoryFolder.GetAbsolutePath();
                    nativeGit = NGit.Api.Git.Open(path);
                }
                else
                {
                    string path = repositoryFolder.GetAbsolutePath();
                    nativeGit = NGit.Api.Git.CloneRepository()
                        .SetURI(settings.Uri)
                        .SetNoCheckout(true)
                        .SetCredentialsProvider(credentialsProvider)
                        .SetDirectory(path).Call();
                }
            }
            catch (EOFException ex)
            {
                throw new InvalidOperationException(
                    "Unable to connect to repository. Run 'git fsck' in the repository to check for possible errors.", ex);
            }
            catch (ArgumentNullException exception)
            {
                throw new ArgumentException(
                    NGitCheckConnectionErrorResolver.INVALID_URI_OR_INSUFFICIENT_ACCESS_RIGHTS_ERROR_MESSAGE, exception);
            }

            return nativeGit;
        }

        public void Fetch()
        {
            var credentialsProvider = new UsernamePasswordCredentialsProvider(_settings.Login, _settings.Password);

            _git.Clean().Call();
            _git.Fetch()
                .SetTimeout(PluginSettings.LoadInt(ConnectionTimeoutInSecondsSectionName, 0))
                .SetCredentialsProvider(credentialsProvider)
                .SetRemoveDeletedRefs(true)
                .Call();
        }

        private static bool IsRepositoryUriChanged(GitRepositoryFolder repositoryFolder,
            ISourceControlConnectionSettingsSource settings)
        {
            return (settings.Uri.ToLower() != repositoryFolder.RepoUri.ToLower()) && repositoryFolder.Exists();
        }

        private GitRepositoryFolder GetLocalRepository(ISourceControlConnectionSettingsSource settings)
        {
            if (_folder.Empty())
            {
                var repositoryFolder = GitRepositoryFolder.Create(settings.Uri);
                _folder.ReplaceWith(repositoryFolder);
                return repositoryFolder;
            }

            GitRepositoryFolder folder = _folder.Single();
            if (!folder.CheckFolder(_folder))
            {
                var repositoryFolder = GitRepositoryFolder.Create(settings.Uri);
                _folder.ReplaceWith(repositoryFolder);
                return repositoryFolder;
            }

            return folder;
        }

        public string GetFileContent(RevisionId revision, string path)
        {
            return GetCommit(revision).GetFileContent(path, _git.GetRepository());
        }

        public RevisionId GetParent(RevisionId changeset)
        {
            return GetCommit(changeset).Parents[0].ConvertToRevisionId();
        }
    }
}
