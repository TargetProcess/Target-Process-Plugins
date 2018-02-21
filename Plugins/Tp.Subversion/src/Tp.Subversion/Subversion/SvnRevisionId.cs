// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Subversion.Subversion
{
    [Serializable]
    public class SvnRevisionId : IComparable
    {
        private long _value;

        public SvnRevisionId()
        {
        }

        public SvnRevisionId(RevisionId revision)
            : this(ConvertToRevision(revision))
        {
        }

        private static long ConvertToRevision(RevisionId revision)
        {
            long revisionId;
            if (!Int64.TryParse(revision.Value, out revisionId))
            {
                revisionId = 0;
            }
            return revisionId;
        }

        public SvnRevisionId(long revision)
        {
            _value = revision;
        }

        public long Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public static implicit operator SvnRevisionId(RevisionId revisionId)
        {
            return new SvnRevisionId(revisionId);
        }

        public static implicit operator SvnRevisionId(long revisionId)
        {
            return new SvnRevisionId(revisionId);
        }

        public static implicit operator RevisionId(SvnRevisionId revisionId)
        {
            return new RevisionId { Value = revisionId.ToString() };
        }

        public int CompareTo(object obj)
        {
            if (obj is RevisionId || obj is SvnRevisionId)
            {
                var thatRevisionId = (SvnRevisionId) obj;
                return Value.CompareTo(thatRevisionId.Value);
            }
            return 0;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
