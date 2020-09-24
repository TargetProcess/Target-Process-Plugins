// 
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Core;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Diff;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Settings;
using VersionControlException = Tp.SourceControl.VersionControlSystem.VersionControlException;

namespace Tp.Tfs.VersionControlSystem
{
    public class TfsVersionControlSystem : VersionControlSystem<ISourceControlConnectionSettingsSource>
    {
        private readonly IDiffProcessor _diffProcessor;
        private readonly ITfsClient _tfsClient;

        public TfsVersionControlSystem(IProfile profile, ISourceControlConnectionSettingsSource settings, ICheckConnectionErrorResolver errorResolver, IActivityLogger logger,
            IDiffProcessor diffProcessor) : base(settings, errorResolver, logger)
        {
            _diffProcessor = diffProcessor;

            var parameters = TfsConnectionHelper.GetTfsConnectionParameters(settings, out var useRest);
            var timeOut = profile.Initialized
                ? TimeSpan.FromMinutes(((TfsPluginProfile) profile.Settings).SynchronizationInterval)
                : TimeSpan.FromSeconds(100);

            _tfsClient = useRest ? (ITfsClient)new TfvcHttpTfsClient(parameters, timeOut) : new TfsClient(parameters, timeOut);
        }

        public override RevisionInfo[] GetRevisions(RevisionRange revisionRange)
        {
            return _tfsClient.GetRevisions(revisionRange.FromChangeset, revisionRange.ToChangeset);
        }

        public override string GetTextFileContent(RevisionId changeset, string path)
        {
            return _tfsClient.GetTextFileContent(changeset, path);
        }

        public override byte[] GetBinaryFileContent(RevisionId changeset, string path)
        {
            return _tfsClient.GetBinaryFileContent(changeset, path);
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

        public override RevisionRange[] GetFromTillHead(RevisionId from, int pageSize)
        {
            return _tfsClient.GetFromTillHead(int.Parse(from.Value), pageSize).ToArray();
        }

        public override RevisionRange[] GetAfterTillHead(RevisionId from, int pageSize)
        {
            return _tfsClient.GetAfterTillHead(from, pageSize).ToArray();
        }

        public override RevisionRange[] GetFromAndBefore(RevisionId from, RevisionId to, int pageSize)
        {
            return _tfsClient.GetFromAndBefore(from, to, pageSize).ToArray();
        }

        public override DiffResult GetDiff(RevisionId changeset, string path)
        {
            try
            {
                return _tfsClient.GetDiff(changeset, _diffProcessor, path);
            }
            catch (Exception ex)
            {
                throw new VersionControlException($"TFS exception: {ex.Message}");
            }
        }
    }
}
