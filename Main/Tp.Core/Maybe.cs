// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Core
{
	public interface IMaybe
	{
		bool HasValue { get; }
	}

	public class Maybe : IMaybe
	{
		public static readonly Maybe Nothing = new Maybe();

		private Maybe(){}

		public static Maybe<T> Just<T>(T value)
		{
			return new Maybe<T>(value);
		}

		public static Maybe<T> Return<T>(T v)
		{
			return Just(v);
		}

		public static Maybe<T> Return<T>(T v, bool nullMeansNothing)
			where T : class
		{
			return nullMeansNothing ? (v != null ? Just(v) : Nothing) : Return(v);
		}

		public static Maybe<TTo> Bind<TTo, TFrom>(Maybe<TFrom> m, Func<TFrom, Maybe<TTo>> f)
		{
			return m.HasValue ? f(m.Value) : Nothing;
		}

		public bool Equals(Maybe other)
		{
			return !ReferenceEquals(null, other);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is Maybe;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public static bool operator ==(Maybe left, Maybe right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Maybe left, Maybe right)
		{
			return !Equals(left, right);
		}

		public bool HasValue
		{
			get { return false; }
		}
	}

	public class Maybe<T> : IMaybe
	{
		internal Maybe(T value)
		{
			HasValue = true;
			Value = value;
		}

		public Maybe()
		{
			HasValue = false;
		}

		public bool HasValue { get; private set; }
		public T Value { get; private set; }

		public static implicit operator Maybe<T>(Maybe nothing)
		{
			return Nothing;
		}

		public static implicit operator Maybe<T>(T value)
		{
			var maybe = value as IMaybe;
			if (maybe != null && !maybe.HasValue)
				return Nothing;
			return new Maybe<T>(value);
		}
	
		public bool Equals(Maybe<T> other)
		{
			if (ReferenceEquals(null, other))
				return false;

			return !HasValue ? !other.HasValue : other.HasValue && Equals(other.Value, Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is Maybe<T>)
				return Equals((Maybe<T>) obj);
			var maybe = obj as IMaybe;
			if (maybe != null && !maybe.HasValue)
				return !HasValue;
			return false;
		}

		public override int GetHashCode()
		{
			return HasValue ? Value.GetHashCode() : 0;
		}

		public static bool operator ==(Maybe<T> left, Maybe<T> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Maybe<T> left, Maybe<T> right)
		{
			return !Equals(left, right);
		}

		private static readonly Maybe<T> Nothing = new Maybe<T>();

		public static Maybe<T> Just<TU>(TU value) where TU : T
		{
			return new Maybe<T>(value);
		}
	}
}