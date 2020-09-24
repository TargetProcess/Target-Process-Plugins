// 
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Globalization;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Tfs.VersionControlSystem
{
    public static class TfvcChangesetRefExtensions
    {
        public static RevisionInfo ToRevisionInfo(this TfvcChangesetRef changeset)
        {
            return new RevisionInfo
            {
                Author = changeset.Author.UniqueName,
                Comment = changeset.Comment,
                Id = new RevisionId { Time = changeset.CreatedDate, Value = changeset.ChangesetId.ToString(CultureInfo.InvariantCulture) },
                Time = changeset.CreatedDate,
                Entries = new RevisionEntryInfo[] {},
                Email = string.Empty,
                TimeCreated = changeset.CreatedDate
            };
        }

        public static RevisionId ToRevisionId(this TfvcChangesetRef changeset)
        {
            return new RevisionId
            {
                Time = changeset.CreatedDate,
                Value = changeset.ChangesetId.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
