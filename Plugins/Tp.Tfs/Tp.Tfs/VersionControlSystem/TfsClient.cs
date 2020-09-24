//
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Git.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common;
using Tp.Core;
using Tp.SourceControl.Diff;
using Tp.SourceControl.VersionControlSystem;
using VersionControlException = Tp.SourceControl.VersionControlSystem.VersionControlException;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace Tp.Tfs.VersionControlSystem
{
    public class TfsClient : ITfsClient
    {
        private readonly VersionControlServer _versionControl;

        private TfsTeamProjectCollection _teamProjectCollection;
        private TeamProject[] _teamProjects;

        public TfsClient(TfsConnectionParameters parameters, TimeSpan maxTimeout)
        {
            _versionControl = GetVersionControl(parameters, maxTimeout);
        }

        ~TfsClient()
        {
            Dispose(false);
        }

        public IEnumerable<RevisionRange> GetFromTillHead(Int32 from, int pageSize)
        {
            return GetChangesetsRanges(from, int.MaxValue, pageSize);
        }

        public IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize)
        {
            return GetChangesetsRanges(int.Parse(revisionId.Value), int.MaxValue, pageSize, true);
        }

        public IEnumerable<RevisionRange> GetFromAndBefore(RevisionId fromRevision, RevisionId toRevision, int pageSize)
        {
            return GetChangesetsRanges(int.Parse(fromRevision.Value), int.Parse(toRevision.Value), pageSize);
        }

        public int? GetLatestChangesetId(int startRevision = 0)
        {
            var changeSetIds = new List<int>();

            foreach (var project in _teamProjects)
            {
                try
                {
                    var changeset = _versionControl.QueryHistory(
                        project.ServerItem,
                        VersionSpec.Latest,
                        0,
                        RecursionType.Full,
                        null,
                        null,
                        VersionSpec.Latest,
                        1,
                        false,
                        false).Cast<Changeset>().FirstOrDefault();

                    if (changeset != null)
                    {
                        if (startRevision >= changeset.ChangesetId)
                            return changeset.ChangesetId;
                        changeSetIds.Add(changeset.ChangesetId);
                    }
                }
                catch (ChangesetNotFoundException)
                {
                }
            }

            return changeSetIds.Any() ? changeSetIds.Max(x => x) : (int?)null;
        }

        private Changeset GetParentCommit(Changeset commit, string path)
        {
            var changeset = GetFirstChangesetBefore(commit.ChangesetId, path);
            if (changeset == null)
            {
                throw new VersionControlException($"No parent Changeset found for path '{path}'");
            }
            return _versionControl.GetChangeset(changeset.ChangesetId);
        }

        public string GetTextFileContent(RevisionId changeset, string path)
        {
            var commit = GetCommit(changeset);
            return GetFileContent(commit, path);
        }

        public byte[] GetBinaryFileContent(RevisionId changeset, string path)
        {
            throw new NotImplementedException();
        }

        public Changeset GetCommit(RevisionId id)
        {
            return _versionControl.GetChangeset(int.Parse(id.Value), true, true);
        }

        public string[] RetrieveAuthors(DateRange dateRange)
        {
            var authors =
                GetChangesets(CreateDateVSpec(dateRange.StartDate.GetValueOrDefault()),
                        CreateDateVSpec(dateRange.EndDate.GetValueOrDefault()))
                    .Select(changeset => changeset.Committer)
                    .Distinct()
                    .ToArray();

            return authors;
        }

        private static VersionSpec CreateDateVSpec(DateTime date)
        {
            //Format is:  D2009-11-16T14:32
            return VersionSpec.ParseSingleSpec(
                string.Format("D{0:yyy}-{0:MM}-{0:dd}T{0:HH}:{0:mm}", date),
                string.Empty);
        }

        public RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset)
        {
            var revisionInfos =
                GetChangesets(VersionSpec.ParseSingleSpec((fromChangeset.Value).ToString(CultureInfo.InvariantCulture), null),
                        VersionSpec.ParseSingleSpec((toChangeset.Value).ToString(CultureInfo.InvariantCulture), null))
                    .Select(changeset => changeset.ToRevisionInfo())
                    .ToArray();

            return revisionInfos;
        }

        public DiffResult GetDiff(RevisionId changeset, IDiffProcessor diffProcessor, string path)
        {
            var commit = GetCommit(changeset);
            return GetDiff(commit, diffProcessor, path);
        }

        private DiffResult GetDiff(Changeset commit, IDiffProcessor diffProcessor, string path)
        {
            var parent = GetParentCommit(commit, path);
            var fileContent = GetTextFileContentSafe(commit, path);
            var previousRevisionFileContent = GetTextFileContentSafe(parent, path);
            var diff = diffProcessor.GetDiff(previousRevisionFileContent, fileContent);
            diff.LeftPanRevisionId = parent.ChangesetId.ToString();
            diff.RightPanRevisionId = commit.ChangesetId.ToString();

            return diff;
        }

        private string GetFileContent(Changeset commit, string path)
        {
            Changeset changeset = _versionControl.GetChangeset(commit.ChangesetId, true, true);
            Change change = changeset.Changes.FirstOrDefault(ch => ch.Item.ServerItem.Equals(path));

            if (change == null)
                return string.Empty;

            if (change.Item.ItemType != ItemType.File)
                return "This is a folder.";

            string content;
            Stream stream = change.Item.DownloadFile();

            using (StreamReader reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }

        private string GetTextFileContentSafe(Changeset commit, string path)
        {
            try
            {
                return GetFileContent(commit, path);
            }
            catch
            {
                return string.Empty;
            }
        }

        private VersionControlServer GetVersionControl(TfsConnectionParameters parameters, TimeSpan sendTimeout)
        {
            VersionControlServer versionControl;

            switch (parameters.TfsCollection)
            {
                case TfsCollection.Project:
                {
                    _teamProjectCollection = new TfsTeamProjectCollection(parameters.TfsCollectionUri,
                        new VssCredentials(new WindowsCredential(parameters.Credential), CredentialPromptType.DoNotPrompt), null,
                        new TfsHttpRetryChannelFactory(sendTimeout));
                    _teamProjectCollection.EnsureAuthenticated();

                    versionControl = _teamProjectCollection.GetService<VersionControlServer>();
                    _teamProjects = versionControl.GetAllTeamProjects(false);

                    break;
                }
                case TfsCollection.TeamProject:
                {
                    _teamProjectCollection = new TfsTeamProjectCollection(parameters.TfsCollectionUri,
                        new VssCredentials(new WindowsCredential(parameters.Credential), CredentialPromptType.DoNotPrompt), null,
                        new TfsHttpRetryChannelFactory(sendTimeout));
                        _teamProjectCollection.EnsureAuthenticated();

                    try
                    {
                        versionControl = _teamProjectCollection.GetService<VersionControlServer>();
                        _teamProjects = new[] { versionControl.GetTeamProject(parameters.TeamProjectName) };
                    }
                    catch (Microsoft.TeamFoundation.VersionControl.Client.VersionControlException)
                    {
                        var gitRepositoryService = _teamProjectCollection.GetService<GitRepositoryService>();
                        var gitRepositories = gitRepositoryService.QueryRepositories(parameters.TeamProjectName);
                        var gitRepository = gitRepositories.Single(gr => gr.Name.Equals(parameters.TeamProjectName));
                        if (gitRepository != null)
                        {
                            throw new VersionControlException(
                                $"Git team project is not supported, use Git plugin with '{gitRepository.RemoteUrl}' instead.");
                        }
                        throw;
                    }
                    break;
                }
                default:
                    throw new Exception("Wrong URI format.");
            }

            return versionControl;
        }

        private Changeset GetFirstChangesetBefore(int before, string path)
        {
           return _versionControl.QueryHistory(
                path,
                VersionSpec.Latest,
                0,
                RecursionType.Full,
                null,
                null,
                VersionSpec.ParseSingleSpec((before - 1).ToString(CultureInfo.InvariantCulture), null),
                1,
                true,
                true).Cast<Changeset>().FirstOrDefault();
        }

        private IEnumerable<Changeset> GetChangesets(VersionSpec fromChangeset, VersionSpec toChangeset)
        {
            List<Changeset> changesets = new List<Changeset>();

            foreach (var teamProject in _teamProjects)
            {
                changesets.AddRange(
                    _versionControl.QueryHistory(
                        teamProject.ServerItem,
                        VersionSpec.Latest,
                        0,
                        RecursionType.Full,
                        null,
                        fromChangeset,
                        toChangeset,
                        int.MaxValue,
                        true,
                        true,
                        false,
                        true).Cast<Changeset>());
            }

            return changesets;
        }

        private IEnumerable<RevisionRange> GetChangesetsRanges(int startChangesetId, int endChangesetId, int pageSize,
            bool skipStartChangeset = false)
        {
            var changesets = new List<RevisionRange>();

            foreach (var teamProject in _teamProjects)
            {
                var pageIncrement = skipStartChangeset ? 2 : 1;
                var from = startChangesetId;
                var to = endChangesetId;

                do
                {
                    var pageResult = _versionControl.QueryHistory(
                        teamProject.ServerItem,
                        VersionSpec.Latest,
                        0,
                        RecursionType.Full,
                        null,
                        VersionSpec.ParseSingleSpec((from).ToString(CultureInfo.InvariantCulture), null),
                        to == int.MaxValue ? null : VersionSpec.ParseSingleSpec((to).ToString(CultureInfo.InvariantCulture), null),
                        pageSize + pageIncrement,
                        true,
                        true,
                        false,
                        true).Cast<Changeset>().ToArray();

                    pageIncrement = 1;

                    if (skipStartChangeset && pageResult.Any() && pageResult.First().ChangesetId == startChangesetId)
                    {
                        pageResult = pageResult.Skip(1).ToArray();
                    }

                    if (pageResult.Any())
                    {
                        if (pageResult.Length > pageSize)
                        {
                            changesets.Add(new RevisionRange(pageResult.First().ToRevisionId(), pageResult[pageSize - 1].ToRevisionId()));
                            from = pageResult.Last().ChangesetId;
                        }
                        else
                        {
                            changesets.Add(new RevisionRange(pageResult.First().ToRevisionId(), pageResult.Last().ToRevisionId()));
                            from = -1;
                        }
                    }
                    else
                    {
                        from = -1;
                    }
                } while (from != -1);
            }

            return changesets;
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _teamProjectCollection?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
