using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
    public class LibgitClientFactory : IGitClientFactory
    {
        public IGitClient Create(ISourceControlConnectionSettingsSource settings, IStorage<GitRepositoryFolder> folderStorage)
        {
            return new LibgitClient(settings, folderStorage);
        }
    }

    public class LibgitClient : IGitClient
    {
        private readonly ISourceControlConnectionSettingsSource _settings;
        private readonly GitRepositoryFolder _repositoryFolder;

        public static CredentialsHandler CreateCredentialsHandler(ISourceControlConnectionSettingsSource settings)
        {
            return (url, fromUrl, types) => new UsernamePasswordCredentials { Username = settings.Login, Password = settings.Password };
        }

        public LibgitClient(ISourceControlConnectionSettingsSource settings, IStorage<GitRepositoryFolder> folderStorage)
        {
            _settings = settings;
            _repositoryFolder = GetRepositoryFolder(folderStorage);
            CloneIfNeeded();
        }

        private GitRepositoryFolder GetRepositoryFolder(IStorage<GitRepositoryFolder> folderStorage)
        {
            var repositoryFolder = GetFolderFromStorage(_settings, folderStorage);
            if (repositoryFolder.Exists() && !_settings.Uri.EqualsIgnoreCase(repositoryFolder.RepoUri))
            {
                repositoryFolder.Delete();
                repositoryFolder = GitRepositoryFolder.Create(_settings.Uri);
                folderStorage.ReplaceWith(repositoryFolder);
            }
            return repositoryFolder;
        }

        private static GitRepositoryFolder GetFolderFromStorage(ISourceControlConnectionSettingsSource settings,
            IStorage<GitRepositoryFolder> folderStorage)
        {
            GitRepositoryFolder folder = folderStorage.SingleOrDefault();
            if (folder == null || !folder.CheckFolder(folderStorage))
            {
                folder = GitRepositoryFolder.Create(settings.Uri);
                folderStorage.ReplaceWith(folder);
            }            
            return folder;
        }

        private void CloneIfNeeded()
        {
            if (!_repositoryFolder.Exists())
            {
                Repository.Clone(_settings.Uri, _repositoryFolder.GetAbsolutePath(), new CloneOptions
                {
                    Checkout = false,
                    CredentialsProvider = CreateCredentialsHandler(_settings)
                });
            }
        }

        public void Fetch()
        {
            using (var repo = new Repository(_repositoryFolder.GetAbsolutePath()))
            {
                foreach (Remote remote in repo.Network.Remotes)
                {
                    IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    LibGit2Sharp.Commands.Fetch(repo, remote.Name, refSpecs, new FetchOptions
                    {
                        Prune = true,
                        CredentialsProvider = CreateCredentialsHandler(_settings)
                    }, "");
                }
            }
        }

        public IEnumerable<RevisionRange> GetFromTillHead(DateTime from, int pageSize)
        {
            return QueryCommits(cs => ToRevisionRange(cs.Where(c => c.Committer.When.UtcDateTime >= from), pageSize));
        }

        public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId from, RevisionId to, int pageSize)
        {
            return QueryCommits(cs =>
            {
                var commits = cs
                    .Where(c => c.Committer.When.UtcDateTime > ((GitRevisionId) from).Time)
                    .Where(c => c.Committer.When.UtcDateTime < ((GitRevisionId) to).Time);
                return ToRevisionRange(commits, pageSize);
            });
        }

        private T QueryCommits<T>(Func<IEnumerable<Commit>, Repository, T> selector)
        {
            return DoInRepo(r =>
            {
                var commits = r.Commits.QueryBy(new CommitFilter { SortBy = CommitSortStrategies.Time, IncludeReachableFrom = r.Refs });
                return selector(commits.Where(c => c.Parents.Count() < 2).OrderBy(c => c.Committer.When.UtcDateTime), r);
            });              
        }

        private T QueryCommits<T>(Func<IEnumerable<Commit>, T> selector)
        {
            return QueryCommits((cs, _) => selector(cs));
        }

        private static IEnumerable<RevisionRange> ToRevisionRange(IEnumerable<Commit> commits, int pageSize)
        {
            return commits
                .ToArray()
                .Split(pageSize)
                .Select(x => x.ToArray())
                .Select(x => new RevisionRange(GetRevision(x.First()), GetRevision(x.Last())))
                .ToArray();
        }

        public IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize)
        {
            return GetFromTillHead(revisionId.Time.Value.AddSeconds(1), pageSize);
        }

        public string[] RetrieveAuthors(DateRange dateRange)
        {
            return QueryCommits(
                cs => cs
                    .Where(c => dateRange.Contains(c.Committer.When.UtcDateTime))
                    .Select(c => c.Author.Name)
                    .Distinct()
                    .ToArray()
            );
        }

        public RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset)
        {
            var from = ((GitRevisionId) fromChangeset).Time;
            var to = ((GitRevisionId)toChangeset).Time;
            var dateRange = new DateRange(from, to);
            return QueryCommits((cs, repo) =>
                cs.Where(c => dateRange.Contains(c.Committer.When.UtcDateTime)).Select(c => new RevisionInfo
                {
                    Id = c.Sha,
                    Time = c.Committer.When.UtcDateTime,
                    TimeCreated = c.Author.When.UtcDateTime,
                    Author = c.Author.Name,
                    Email = c.Author.Email,                    
                    Comment = c.Message.TrimEnd('\n'),                    
                    Entries = repo.Diff.Compare<TreeChanges>(c.Parents.FirstOrDefault()?.Tree, c.Tree).Select(x => new RevisionEntryInfo
                    {
                        Action = GetAction(x), 
                        Path = GetPath(x).Replace('\\', '/')
                    }).ToArray()
                }).ToArray()
            );
        }

        private static string GetPath(TreeEntryChanges treeEntryChanges)
        {
            if (treeEntryChanges.Status == ChangeKind.Renamed || treeEntryChanges.Status == ChangeKind.Copied)
            {
                return $"{treeEntryChanges.OldPath} -> {treeEntryChanges.Path}";
            }
            return treeEntryChanges.Path ?? treeEntryChanges.OldPath;
        }

        private static FileActionEnum GetAction(TreeEntryChanges treeEntryChanges)
        {
            switch (treeEntryChanges.Status)
            {
                case ChangeKind.Added: return FileActionEnum.Add;
                case ChangeKind.Deleted: return FileActionEnum.Delete;
                case ChangeKind.Renamed: return FileActionEnum.Rename;

                case ChangeKind.Modified:
                case ChangeKind.Copied:
                    return FileActionEnum.Modify;
                
                default: return FileActionEnum.None;
            }
        }

        public RevisionId GetParent(RevisionId revision)
        {
            return DoInRepo(r => GetRevision(r.Lookup<Commit>(revision.Value).Parents.First()));
        }

        public string GetFileContent(RevisionId revision, string path)
        {
            return DoInRepo(r => ((Blob) r.Lookup<Commit>(revision.Value)[path].Target).GetContentText());            
        }

        private T DoInRepo<T>(Func<Repository, T> action)
        {
            using (var repo = new Repository(_repositoryFolder.GetAbsolutePath()))
            {
                return action(repo);
            }
        }

        private static RevisionId GetRevision(Commit commit)
        {
            return new RevisionId { Value = commit.Sha, Time = commit.Committer.When.UtcDateTime };
        }
    }
}
