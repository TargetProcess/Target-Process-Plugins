// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Integration.Messages
{
	/// <summary>
	/// A value object for holding profile name value.
	/// </summary>
	[Serializable]
	public class ProfileName
	{
		public ProfileName()
			: this(string.Empty)
		{
		}

		public ProfileName(string profileName)
		{
			Value = profileName;
		}

		public string Value { get; set; }

		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(Value); }
		}

		public bool Equals(ProfileName other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ProfileName)) return false;
			return Equals((ProfileName) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public static implicit operator ProfileName(string profileName)
		{
			return new ProfileName(profileName);
		}

		public static bool operator ==(ProfileName left, ProfileName right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ProfileName left, ProfileName right)
		{
			return !(left == right);
		}
	}
}