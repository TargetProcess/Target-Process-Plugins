using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercurial;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Mercurial.VersionControlSystem
{
    public static class ChangesetExtensions
    {
        public static RevisionInfo ToRevisionInfo(this Changeset changeset)
        {
            return new RevisionInfo
            {
                Author = changeset.AuthorName,
                Comment = changeset.CommitMessage,
                Id = new RevisionId() {Time = changeset.Timestamp, Value = changeset.Hash},
                Time = changeset.Timestamp,
                Entries = changeset.PathActions
                    .Select(pa => new RevisionEntryInfo() { Path = pa.Path, Action = pa.Action.ToFileAction() }).ToArray(),
                Email = changeset.AuthorEmailAddress,
                TimeCreated = changeset.Timestamp
            };
        }

        public static RevisionId ToRevisionId(this Changeset changeset)
        {
            return new RevisionId
            {
                Time = changeset.Timestamp,
                Value = changeset.Hash
            };
        }
    }
}
