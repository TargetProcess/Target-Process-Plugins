using System;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Perforce.VersionControlSystem
{
    [Serializable]
    public class P4RevisionId : IComparable
    {
        public P4RevisionId()
        {
        }

        public P4RevisionId(RevisionId revision)
            : this(ConvertToRevision(revision))
        {
        }

        private static int ConvertToRevision(RevisionId revision)
        {
            if (!int.TryParse(revision.Value, out var revisionId))
            {
                revisionId = 0;
            }
            return revisionId;
        }

        public P4RevisionId(int revision)
        {
            Value = revision;
        }

		public int Value { get; set; }

		public static implicit operator P4RevisionId(RevisionId revisionId)
        {
            return new P4RevisionId(revisionId);
        }

        public static implicit operator P4RevisionId(int revisionId)
        {
            return new P4RevisionId(revisionId);
        }

        public static implicit operator RevisionId(P4RevisionId revisionId)
        {
            return new RevisionId { Value = revisionId.ToString() };
        }

        public int CompareTo(object obj)
        {
            if (obj is RevisionId || obj is P4RevisionId)
            {
                var thatRevisionId = (P4RevisionId)obj;
                return Value.CompareTo(thatRevisionId.Value);
            }
            return 0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
