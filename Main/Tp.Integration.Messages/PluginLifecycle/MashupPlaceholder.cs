// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
namespace Tp.Integration.Messages.PluginLifecycle
{
	public class MashupPlaceholder
	{
		public MashupPlaceholder(string name)
		{
			Value = name;
		}

		public string Value { get; set; }

		public bool Equals(MashupPlaceholder other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (MashupPlaceholder)) return false;
			return Equals((MashupPlaceholder) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public static implicit operator MashupPlaceholder(string placeholder)
		{
			return new MashupPlaceholder(placeholder);
		}

		public override string ToString()
		{
			return Value;
		}

		public static bool operator ==(MashupPlaceholder left, MashupPlaceholder right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(MashupPlaceholder left, MashupPlaceholder right)
		{
			return !(left == right);
		}
	}
}