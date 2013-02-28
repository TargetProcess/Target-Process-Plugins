// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Core
{
	using System;
	using System.Text.RegularExpressions;

	public class TypeNameWithoutVersion
		: IEquatable<TypeNameWithoutVersion>
	{
		public TypeNameWithoutVersion(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			Value = Regex.Replace(value, @", Version=\d+.\d+.\d+.\d+", string.Empty);
			Value = Regex.Replace(Value, @", Culture=\w+", string.Empty);
			Value = Regex.Replace(Value, @", PublicKeyToken=\w+", string.Empty);
		}

		public TypeNameWithoutVersion(Type type)
			: this(type.AssemblyQualifiedName)
		{
		}

		public string Value { get; private set; }

		public static bool operator ==(TypeNameWithoutVersion left, TypeNameWithoutVersion right)
		{
			return Equals(left, right);
		}

		public static bool operator ==(string left, TypeNameWithoutVersion right)
		{
			return Equals(right, left);
		}

		public static bool operator !=(string left, TypeNameWithoutVersion right)
		{
			return !Equals(right, left);
		}

		public static bool operator ==(TypeNameWithoutVersion left, string right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TypeNameWithoutVersion left, string right)
		{
			return !Equals(left, right);
		}

		public static bool operator !=(TypeNameWithoutVersion left, TypeNameWithoutVersion right)
		{
			return !Equals(left, right);
		}

		public bool Equals(TypeNameWithoutVersion other)
		{
			return Equals(this, other);
		}

		public override bool Equals(object obj)
		{
			return Equals(this, obj as string) || Equals(this, obj as TypeNameWithoutVersion);
		}

		public override int GetHashCode()
		{
			return Value != null ? Value.GetHashCode() : 0;
		}

		private static bool Equals(TypeNameWithoutVersion left, string other)
		{
			if (ReferenceEquals(left, null))
			{
				return ReferenceEquals(other, null);
			}

			return left.Value == other;
		}

		private static bool Equals(TypeNameWithoutVersion left, TypeNameWithoutVersion other)
		{
			if (ReferenceEquals(left, null))
			{
				return ReferenceEquals(other, null);
			}

			return left.Value == other.Value;
		}

		public static TypeNameWithoutVersion Create<T>()
		{
			return new TypeNameWithoutVersion(typeof(T));
		}

		public override string ToString()
		{
			return Value;
		}
	}
}