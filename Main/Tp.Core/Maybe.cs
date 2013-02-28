// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Diagnostics;

namespace Tp.Core
{
	public interface IMaybe
	{
		bool HasValue { get; }
		object Value { get; }
	}

	public struct Maybe : IMaybe, IEquatable<Maybe>
	{
		public static readonly Maybe Nothing=default(Maybe);

		[DebuggerStepThrough]
		public static Maybe<T> Just<T>(T value)
		{
			return new Maybe<T>(value);
		}

		[DebuggerStepThrough]
		public static Maybe<T> Return<T>(T v)
		{
			return Just(v);
		}

		[DebuggerStepThrough]
		public static Maybe<T> ReturnIfNotNull<T>(T v)
			where T : class
		{
			return v == null ? Nothing : Just(v);
		}


		[DebuggerStepThrough]
		public static Maybe<TTo> Bind<TTo, TFrom>(Maybe<TFrom> m, Func<TFrom, Maybe<TTo>> f)
		{
			return m.HasValue ? f(m.Value) : Nothing;
		}

		[DebuggerStepThrough]
		public override int GetHashCode()
		{
			return 0;
		}

		[DebuggerStepThrough]
		public static bool operator ==(Maybe left, Maybe right)
		{
			return Equals(left, right);
		}

		[DebuggerStepThrough]
		public static bool operator !=(Maybe left, Maybe right)
		{
			return !Equals(left, right);
		}

		public bool HasValue
		{
			[DebuggerStepThrough]
			get { return false; }
		}
		[DebuggerStepThrough]
		public static Maybe<TTo> Cast<T, TTo>(T o) where TTo : T
		{
			return o is TTo ? Just((TTo)o) : Nothing;
		}

		public object Value
		{
			get { throw new NotSupportedException(); }
		}

		[DebuggerStepThrough]
		public bool Equals(Maybe other)
		{
			return true;
		}

		[DebuggerStepThrough]
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (Maybe)) return false;
			return Equals((Maybe) obj);
		}

		[DebuggerStepThrough]
		public static Maybe<T> Try<T>(Func<T> action)
		{
			return Either.Try(action).Switch(Return, _ => Nothing);
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			return "Nothing";
		}
	}

	public struct Maybe<T> : IMaybe
	{
		private readonly bool _hasValue;
		private readonly T _value;

		public bool HasValue
		{
			[DebuggerStepThrough]
			get { return _hasValue; }
		}

		object IMaybe.Value
		{
			[DebuggerStepThrough]
			get
			{
				if(!HasValue)
				{
					throw new InvalidOperationException("Cannot get value from Nothing");
				}
				return _value;
			}
		}

		public T Value
		{
			[DebuggerStepThrough]
			get { return _value; }
		}

		[DebuggerStepThrough]
		public static implicit operator Maybe<T>(Maybe nothing)
		{
			return Nothing;
		}

		[DebuggerStepThrough]
		public static implicit operator Maybe<T>(T value)
		{
			var maybe = value as IMaybe;
			if (maybe != null && !maybe.HasValue)
				return Nothing;
			return new Maybe<T>(value);
		}

		[DebuggerStepThrough]
		public bool Equals(Maybe<T> other)
		{
			return (!HasValue && !other.HasValue) || (other.HasValue && Equals(other.Value, Value));
		}

		[DebuggerStepThrough]
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj is Maybe<T>)
				return Equals((Maybe<T>) obj);
			var maybe = obj as IMaybe;
			if (maybe != null && !maybe.HasValue)
				return !HasValue;
			return false;
		}

		[DebuggerStepThrough]
		public override int GetHashCode()
		{
			return HasValue ? Value.GetHashCode() : 0;
		}

		[DebuggerStepThrough]
		public static bool operator ==(Maybe<T> left, Maybe<T> right)
		{
			return Equals(left, right);
		}

		[DebuggerStepThrough]
		public static bool operator !=(Maybe<T> left, Maybe<T> right)
		{
			return !Equals(left, right);
		}

		public static readonly Maybe<T> Nothing=Maybe.Nothing;

		[DebuggerStepThrough]
		internal Maybe(T value)
		{
			_value = value;
			_hasValue = true;
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			return HasValue? "Just<{0}>( {1} )".Fmt(typeof(T).Name,Value):"Nothing";
		}

	}
}
