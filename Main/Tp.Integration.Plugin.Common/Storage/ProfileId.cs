// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Plugin.Common.Storage
{
	public class ProfileId
	{
		public ProfileId(int profileId)
		{
			Value = profileId;
		}

		public int Value { get; set; }

		public bool Equals(ProfileId other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ProfileId)) return false;
			return Equals((ProfileId) obj);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static implicit operator ProfileId(int profileId)
		{
			return new ProfileId(profileId);
		}

		public static bool operator ==(ProfileId left, ProfileId right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ProfileId left, ProfileId right)
		{
			return !(left == right);
		}
	}
}