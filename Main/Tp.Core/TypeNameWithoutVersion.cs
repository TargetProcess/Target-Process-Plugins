// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Core
{
	public class TypeNameWithoutVersion
	{
		private TypeNameWithoutVersion(string value)
		{
			Value = value.GetTypeNameWithoutVersion();
		}

		private TypeNameWithoutVersion(Type type)
			: this(type.AssemblyQualifiedName)
		{
		}

		public string Value { get; private set; }

		public static implicit operator TypeNameWithoutVersion(string typeName)
		{
			return new TypeNameWithoutVersion(typeName);
		}

		public static implicit operator TypeNameWithoutVersion(Type type)
		{
			return new TypeNameWithoutVersion(type);
		}

		public static bool operator ==(TypeNameWithoutVersion left, TypeNameWithoutVersion right)
		{
			return left.Equals(right);
		}

		public static bool operator ==(string left, TypeNameWithoutVersion right)
		{
			return ((TypeNameWithoutVersion)left).Equals(right);
		}

		public static bool operator !=(string left, TypeNameWithoutVersion right)
		{
			return ((TypeNameWithoutVersion)left).Equals(right);
		}

		public static bool operator ==(TypeNameWithoutVersion left, string right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TypeNameWithoutVersion left, string right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TypeNameWithoutVersion left, TypeNameWithoutVersion right)
		{
			return !(left == right);
		}

		public bool Equals(TypeNameWithoutVersion other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (TypeNameWithoutVersion)) return false;
			return Equals((TypeNameWithoutVersion) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return Value;
		}
	}
}