//
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections.Generic;
using Tp.Core;
using Tp.SourceControl.Diff;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Tfs.VersionControlSystem
{
    public interface ITfsClient : IDisposable
    {
        string[] RetrieveAuthors(DateRange dateRange);
        int? GetLatestChangesetId(int startRevision = 0);
        RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset);
        IEnumerable<RevisionRange> GetFromTillHead(Int32 from, int pageSize);
        IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize);
        IEnumerable<RevisionRange> GetFromAndBefore(RevisionId fromRevision, RevisionId toRevision, int pageSize);
        DiffResult GetDiff(RevisionId changeset, IDiffProcessor diffProcessor, string path);
        string GetTextFileContent(RevisionId changeset, string path);
        byte[] GetBinaryFileContent(RevisionId changeset, string path);
    }
}
