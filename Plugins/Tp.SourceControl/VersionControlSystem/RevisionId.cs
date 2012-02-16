//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;

namespace Tp.SourceControl.VersionControlSystem
{
	[DataContract]
	[Serializable]
	public class RevisionId
	{
		[DataMember]
		public string Value { get; set; }

		public static implicit operator RevisionId(string revisionId)
		{
			return new RevisionId {Value = revisionId};
		}

		public override string ToString()
		{
			return Value;
		}

		[DataMember]
		public DateTime? Time { get; set; }

		public bool Equals(RevisionId other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof (RevisionId))
			{
				return false;
			}
			return Equals((RevisionId) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public static bool operator ==(RevisionId left, RevisionId right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RevisionId left, RevisionId right)
		{
			return !Equals(left, right);
		}
	}
}