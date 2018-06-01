// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Diff;
using Tp.SourceControl.VersionControlSystem;
using VersionControlException = Tp.SourceControl.VersionControlSystem.VersionControlException;
using Microsoft.TeamFoundation.VersionControl.Client;
using Tp.SourceControl.Settings;

namespace Tp.Tfs.VersionControlSystem
{
    public class TfsVersionControlSystem : VersionControlSystem<ISourceControlConnectionSettingsSource>
    {
        private readonly IDiffProcessor _diffProcessor;
        private readonly TfsClient _tfsClient;

        public TfsVersionControlSystem(ISourceControlConnectionSettingsSource settings, ICheckConnectionErrorResolver errorResolver, IActivityLogger logger,
            IDiffProcessor diffProcessor) : base(settings, errorResolver, logger)
        {
            _diffProcessor = diffProcessor;
            _tfsClient = new TfsClient(settings);
        }

        public override RevisionInfo[] GetRevisions(RevisionRange revisionRange)
        {
            return _tfsClient.GetRevisions(revisionRange.FromChangeset, revisionRange.ToChangeset);
        }

        public override string GetTextFileContent(RevisionId changeset, string path)
        {
            var commit = _tfsClient.GetCommit(changeset);

            return GetFileContent(commit, path);
        }

        public override byte[] GetBinaryFileContent(RevisionId changeset, string path)
        {
            throw new NotImplementedException();
        }

        public override void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors)
        {
            TfsRevisionId revisionId = revision;
            if (int.Parse(revisionId.Value) <= 0)
            {
                _errorResolver.HandleConnectionError(new InvalidRevisionException(), errors);
            }
        }

        public override string[] RetrieveAuthors(DateRange dateRange)
        {
            return _tfsClient.RetrieveAuthors(dateRange);
        }

        public override RevisionRange[] GetFromTillHead(RevisionId @from, int pageSize)
        {
            return _tfsClient.GetFromTillHead(int.Parse(from.Value), pageSize).ToArray();
        }

        public override RevisionRange[] GetAfterTillHead(RevisionId @from, int pageSize)
        {
            return _tfsClient.GetAfterTillHead(from, pageSize).ToArray();
        }

        public override RevisionRange[] GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize)
        {
            return _tfsClient.GetFromAndBefore(from, to, pageSize).ToArray();
        }

        public override DiffResult GetDiff(RevisionId changeset, string path)
        {
            try
            {
                var commit = _tfsClient.GetCommit(changeset);
                var parent = _tfsClient.GetParentCommit(commit, path);
                return GetDiff(path, parent, commit);
            }
            catch (Exception ex)
            {
                throw new VersionControlException($"TFS exception: {ex.Message}");
            }
        }

        private DiffResult GetDiff(string path, Changeset prevCommmit, Changeset commit)
        {
            var fileContent = GetTextFileContentSafe(commit, path);
            var previousRevisionFileContent = GetTextFileContentSafe(prevCommmit, path);
            var diff = _diffProcessor.GetDiff(previousRevisionFileContent, fileContent);

            diff.LeftPanRevisionId = prevCommmit.ChangesetId.ToString();
            diff.RightPanRevisionId = commit.ChangesetId.ToString();

            return diff;
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

        private string GetFileContent(Changeset commit, string path)
        {
            return _tfsClient.GetFileContent(commit, path);
        }
    }
}
