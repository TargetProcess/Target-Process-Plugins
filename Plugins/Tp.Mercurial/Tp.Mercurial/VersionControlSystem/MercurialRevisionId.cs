// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Mono.Unix.Native;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Mercurial.VersionControlSystem
{
	[Serializable]
	public class MercurialRevisionId : IComparable
	{
		public static readonly DateTime UtcTimeMin = NativeConvert.ToDateTime(0);
		public static readonly DateTime UtcTimeMax = new DateTime(2038, 01, 19);

		public MercurialRevisionId()
		{
		}

        public MercurialRevisionId(RevisionId revisionId)
		{
			Time = (!revisionId.Time.HasValue || revisionId.Time.Value < UtcTimeMin) ? UtcTimeMin : revisionId.Time.Value;
			Value = revisionId.Value;
		}

		public DateTime Time { get; set; }
		public string Value { get; set; }

		public static implicit operator MercurialRevisionId(RevisionId revisionId)
		{
            return new MercurialRevisionId(revisionId);
		}

        public static implicit operator RevisionId(MercurialRevisionId revisionId)
		{
			return new RevisionId {Value = revisionId.Value, Time = revisionId.Time};
		}

		public int CompareTo(object obj)
		{
            if (obj is RevisionId || obj is MercurialRevisionId)
			{
                var thatRevisionId = (MercurialRevisionId)obj;
				return Time.CompareTo(thatRevisionId.Time);
			}
			return 0;
		}

		public override string ToString()
		{
			return string.Format("RevisionId: {0}, Date: {1}", Value, Time);
		}
	}
}