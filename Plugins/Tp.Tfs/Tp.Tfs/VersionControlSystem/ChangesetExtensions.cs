using System.Globalization;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Tfs.VersionControlSystem
{
    public static class ChangesetExtensions
    {
        public static RevisionInfo ToRevisionInfo(this Changeset changeset)
        {
            return new RevisionInfo
            {
                Author = changeset.Owner,
                Comment = changeset.Comment,
                Id = new RevisionId { Time = changeset.CreationDate, Value = changeset.ChangesetId.ToString(CultureInfo.InvariantCulture) },
                Time = changeset.CreationDate,
                Entries = changeset.Changes
                    .Select(change => new RevisionEntryInfo { Path = change.Item.ServerItem, Action = ChangesetActionActionExtensions.ToFileAction(change) }).ToArray(),
                Email = string.Empty,
                TimeCreated = changeset.CreationDate
            };
        }

        public static RevisionId ToRevisionId(this Changeset changeset)
        {
            return new RevisionId
            {
                Time = changeset.CreationDate,
                Value = changeset.ChangesetId.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
