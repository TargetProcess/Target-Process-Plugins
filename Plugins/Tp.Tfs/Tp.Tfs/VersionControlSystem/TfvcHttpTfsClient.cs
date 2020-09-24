//
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tp.Core;
using Tp.SourceControl.Diff;
using Tp.SourceControl.VersionControlSystem;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace Tp.Tfs.VersionControlSystem
{
    public class TfvcHttpTfsClient : ITfsClient
    {
        private readonly TfvcHttpClient _httpVersionControl;
        private TeamProjectReference _teamProject;

        public TfvcHttpTfsClient(TfsConnectionParameters parameters, TimeSpan sendTimeout)
        {
            _httpVersionControl = GetVersionControl(parameters, sendTimeout);
        }

        ~TfvcHttpTfsClient()
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

        private string ItemPath => $"$/{(_teamProject != null ? _teamProject.Name : string.Empty)}";

        public int? GetLatestChangesetId(int startRevision = 0)
        {
            try
            {
                return _httpVersionControl
                    .GetChangesetsAsync(_teamProject?.Name, 0, 0, 1, null, new TfvcChangesetSearchCriteria { ItemPath = ItemPath }).Result
                    .FirstOrDefault()?.ChangesetId;
            }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    if (exception is TimeoutException timeoutException)
                    {
                        throw new TimeoutException(timeoutException.Message);
                    }
                    if (exception is ProjectDoesNotExistWithNameException projectDoesNotExistWithNameException)
                    {
                        throw new ProjectDoesNotExistWithNameException(projectDoesNotExistWithNameException.Message);
                    }
                    if (exception is VssServiceException vssServiceException)
                    {
                        throw new VssServiceException(vssServiceException.Message);
                    }
                }
                throw;
            }
        }

        public string GetTextFileContent(RevisionId changeset, string path)
        {
            var commit = GetCommit(changeset);
            return GetFileContent(commit, path).Value;
        }

        public byte[] GetBinaryFileContent(RevisionId changeset, string path)
        {
            throw new NotImplementedException();
        }

        public TfvcChangeset GetCommit(RevisionId id)
        {
            try
            {
                return _httpVersionControl.GetChangesetAsync(_teamProject?.Name, int.Parse(id.Value)).Result;
            }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    if (exception is TimeoutException timeoutException)
                    {
                        throw new TimeoutException(timeoutException.Message);
                    }
                    if (exception is ProjectDoesNotExistWithNameException projectDoesNotExistWithNameException)
                    {
                        throw new ProjectDoesNotExistWithNameException(projectDoesNotExistWithNameException.Message);
                    }
                    if (exception is VssServiceException vssServiceException)
                    {
                        throw new VssServiceException(vssServiceException.Message);
                    }
                }
                throw;
            }
        }

        public string[] RetrieveAuthors(DateRange dateRange)
        {
            return new string[]{};
            /*var authors =
                GetChangesets(CreateDateVSpec(dateRange.StartDate.GetValueOrDefault()),
                        CreateDateVSpec(dateRange.EndDate.GetValueOrDefault()))
                    .Select(changeset => changeset.Committer)
                    .Distinct()
                    .ToArray();

            return authors*/;
        }

        public RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset)
        {
            try
            {
                var revisionInfos = new List<RevisionInfo>();

                var tfvcChangesetRefs = GetChangesets(int.Parse(fromChangeset.Value),int.Parse(toChangeset.Value));

                foreach (var changesetRef in tfvcChangesetRefs)
                {
                    int skip = 0, pageSize = 100;
                    var changes = new List<RevisionEntryInfo>();
                    List<TfvcChange> tfvcChanges;

                    do
                    {
                        tfvcChanges = _httpVersionControl.GetChangesetChangesAsync(changesetRef.ChangesetId, skip, pageSize).Result;
                        changes.AddRange(tfvcChanges
                            .Select(change => new RevisionEntryInfo { Path = change.Item.Path, Action = change.ToFileAction() }).ToArray());
                        skip += pageSize;
                    } while (tfvcChanges.Any());

                    var revisionInfo = changesetRef.ToRevisionInfo();
                    revisionInfo.Entries = changes.ToArray();
                    revisionInfos.Add(revisionInfo);
                }

                return revisionInfos.ToArray();
            }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    if (exception is TimeoutException timeoutException)
                    {
                        throw new TimeoutException(timeoutException.Message);
                    }
                    if (exception is ProjectDoesNotExistWithNameException projectDoesNotExistWithNameException)
                    {
                        throw new ProjectDoesNotExistWithNameException(projectDoesNotExistWithNameException.Message);
                    }
                    if (exception is VssServiceException vssServiceException)
                    {
                        throw new VssServiceException(vssServiceException.Message);
                    }
                }
                throw;
            }
        }

        public DiffResult GetDiff(RevisionId changeset, IDiffProcessor diffProcessor, string path)
        {
            var commit = GetCommit(changeset);
            return GetDiff(commit, diffProcessor, path);
        }

        private DiffResult GetDiff(TfvcChangeset commit, IDiffProcessor diffProcessor, string path)
        {
            var fileContent = GetTextFileContentSafe(commit, path);
            var previousRevisionFileContent = GetTextFileContentSafe(commit, path, TfvcVersionOption.Previous);
            var diff = diffProcessor.GetDiff(previousRevisionFileContent.Value, fileContent.Value);

            diff.LeftPanRevisionId = previousRevisionFileContent.Key?.ChangesetVersion.ToString() ?? int.MinValue.ToString();
            diff.RightPanRevisionId = fileContent.Key?.ChangesetVersion.ToString() ?? int.MinValue.ToString();

            return diff;
        }

        private KeyValuePair<TfvcItem, string> GetFileContent(TfvcChangeset commit, string path, TfvcVersionOption versionOption = TfvcVersionOption.None)
        {
            var content = string.Empty;

            try
            {
                var tfvcItem = _httpVersionControl.GetItemAsync(path, null, false, null, VersionControlRecursionType.None,
                    new TfvcVersionDescriptor { Version = commit.ChangesetId.ToString(), VersionOption = versionOption }).Result;

                if (tfvcItem != null)
                {
                    if (tfvcItem.IsFolder)
                    {
                        content = "This is a folder.";
                    }
                    else
                    {
                        var stream = _httpVersionControl.GetItemTextAsync(path, null, true, null, VersionControlRecursionType.None,
                            new TfvcVersionDescriptor { Version = tfvcItem.ChangesetVersion.ToString() }).Result;

                        using (var reader = new StreamReader(stream, Encoding.GetEncoding(tfvcItem.Encoding)))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
                }
                return new KeyValuePair<TfvcItem, string>(tfvcItem, content);
            }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    if (exception is VssServiceException vssServiceException)
                    {
                        return new KeyValuePair<TfvcItem, string>(null, vssServiceException.Message);
                    }
                }
            }
            catch (Exception e)
            {
                return new KeyValuePair<TfvcItem, string>(null, e.Message);
            }

            return new KeyValuePair<TfvcItem, string>(null, string.Empty);
        }

        private KeyValuePair<TfvcItem, string> GetTextFileContentSafe(TfvcChangeset commit, string path, TfvcVersionOption versionOption = TfvcVersionOption.None)
        {
            try
            {
                return GetFileContent(commit, path, versionOption);
            }
            catch
            {
                return new KeyValuePair<TfvcItem, string>(null, string.Empty);
            }
        }

        private TfvcHttpClient GetVersionControl(TfsConnectionParameters parameters, TimeSpan sendTimeout)
        {
            TfvcHttpClient httpVersionControl = null;

            switch (parameters.TfsCollection)
            {
                case TfsCollection.Project:
                    {
                    var vssConnection = new VssConnection(parameters.TfsCollectionUri,
                        new VssCredentials(new WindowsCredential(parameters.Credential), CredentialPromptType.DoNotPrompt),
                        new VssClientHttpRequestSettings { SendTimeout = sendTimeout });
                    vssConnection.ConnectAsync().SyncResult();

                    httpVersionControl = vssConnection.GetClient<TfvcHttpClient>();

                    break;
}
                case TfsCollection.TeamProject:
                    {
                    var vssConnection = new VssConnection(parameters.TfsCollectionUri,
                        new VssCredentials(new WindowsCredential(parameters.Credential), CredentialPromptType.DoNotPrompt),
                        new VssClientHttpRequestSettings { SendTimeout = sendTimeout });
                    vssConnection.ConnectAsync().SyncResult();

                    try
                    {
                        var pc = vssConnection.GetClient<ProjectHttpClient>();
                        _teamProject = pc.GetProject(parameters.TeamProjectName).Result;

                        httpVersionControl = vssConnection.GetClient<TfvcHttpClient>();
                    }
                    catch (AggregateException aggregateException)
                    {
                        foreach (var exception in aggregateException.InnerExceptions)
                        {
                            if (exception is ProjectDoesNotExistWithNameException vssServiceException)
                            {
                                throw vssServiceException;
                            }
                        }
                    }

                    break;
                }
                default:
                    throw new Exception("Wrong URI format.");
            }

            return httpVersionControl;
        }

        private IEnumerable<TfvcChangesetRef> GetChangesets(int fromChangeset, int toChangeset)
        {
            return _httpVersionControl.GetChangesetsAsync(_teamProject?.Name, Int32.MaxValue, null, null, null,
                new TfvcChangesetSearchCriteria
                {
                    ItemPath = ItemPath,
                    FromId = fromChangeset,
                    ToId = toChangeset
                }).Result;
        }

        private IEnumerable<RevisionRange> GetChangesetsRanges(int startChangesetId, int endChangesetId, int pageSize,
            bool skipStartChangeset = false)
        {
            try
            {
                var changesets = new List<RevisionRange>();

                startChangesetId = skipStartChangeset ? startChangesetId + 1 : startChangesetId;

                var firstChangeref = _httpVersionControl.GetChangesetsAsync(_teamProject?.Name, 0, 0, 1, null,
                    new TfvcChangesetSearchCriteria
                    {
                        ItemPath = ItemPath,
                        ToId = endChangesetId == Int32.MaxValue ? 0 : endChangesetId
                    }).Result.FirstOrDefault();

                while (firstChangeref != null && firstChangeref.ChangesetId >= startChangesetId && firstChangeref.ChangesetId <= endChangesetId)
                {
                    var nextChangeref = _httpVersionControl.GetChangesetsAsync(_teamProject?.Name, 0, pageSize - 1, 1, null,
                        new TfvcChangesetSearchCriteria
                        {
                            ItemPath = ItemPath,
                            FromId = startChangesetId,
                            ToId = firstChangeref.ChangesetId
                        }).Result.FirstOrDefault();

                    if (nextChangeref == null)
                    {
                        var lastChangeref = _httpVersionControl.GetChangesetsAsync(_teamProject?.Name, 0, 0, 1, "id asc",
                                new TfvcChangesetSearchCriteria { ItemPath = ItemPath, FromId = startChangesetId }).Result
                            .FirstOrDefault();
                        changesets.Add(new RevisionRange(lastChangeref.ToRevisionId(), firstChangeref.ToRevisionId()));
                        break;
                    }

                    changesets.Add(new RevisionRange(nextChangeref.ToRevisionId(), firstChangeref.ToRevisionId()));

                    firstChangeref = _httpVersionControl.GetChangesetsAsync(_teamProject?.Name, 0, 1, 1, null,
                        new TfvcChangesetSearchCriteria
                        {
                            ItemPath = ItemPath,
                            FromId = startChangesetId,
                            ToId = nextChangeref.ChangesetId
                        }).Result.FirstOrDefault();
                }

                return changesets;
            }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    if (exception is TimeoutException timeoutException)
                    {
                        throw new TimeoutException(timeoutException.Message);
                    }
                    if (exception is ProjectDoesNotExistWithNameException projectDoesNotExistWithNameException)
                    {
                        throw new ProjectDoesNotExistWithNameException(projectDoesNotExistWithNameException.Message);
                    }
                    if (exception is VssServiceException vssServiceException)
                    {
                         throw new VssServiceException(vssServiceException.Message);
                    }
                }
                throw;
            }
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
                _httpVersionControl?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
