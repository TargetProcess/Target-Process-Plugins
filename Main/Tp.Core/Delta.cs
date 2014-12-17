// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using System;

namespace Tp.Core
{
	public class Delta
	{
		private readonly object _original;
		private readonly object _changed;
		private readonly IEnumerable<string> _changedFields;
		protected Delta(object original, object changed, IEnumerable<string> changedFields)
		{
			_original = original;
			_changed = changed;
			_changedFields = changedFields;
		}

		public object Original
		{
			get { return _original; }
		}

		public object Changed
		{
			get { return _changed; }
		}

		public IEnumerable<string> ChangedFields
		{
			get { return _changedFields; }
		}
		
		public override string ToString()
		{
			return "Delta. Original: {0}, Changed: {1}".Fmt(Original, Changed);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Delta);
		}

		private bool Equals(Delta other)
		{
			if (other == null)
			{
				return false;
			}

			return Equals(_original, other._original) && Equals(_changed, other._changed);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((_original != null ? _original.GetHashCode() : 0) * 397) ^ (_changed != null ? _changed.GetHashCode() : 0);
			}
		}

		public static Delta<T> Create<T>(T original, T changed, IEnumerable<string> changedFields = null)
		{
			return new Delta<T>(original, changed, changedFields ?? Enumerable.Empty<string>());
		}
	}

	public class Delta<TData> : Delta
	{
		public Delta(TData original, TData changed, IEnumerable<string> changedFields) : base(original, changed, changedFields){}

		public new TData Original
		{
			get { return (TData) base.Original; }
		}

		public new TData Changed
		{
			get { return (TData) base.Changed; }
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Delta<TData>);
		}

		private bool Equals(Delta<TData> other)
		{
			if (other == null)
			{
				return false;
			}

			return Equals(Changed, other.Changed);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return Changed != null ? Changed.GetHashCode() : 0;
			}
		}
	}

	public static class DeltaExtensions
	{
		public static string Dump(this IEnumerable<Delta> deltas)
		{
			return string.Join(Environment.NewLine, deltas.Select(x => x.ToString()));
		}

		public static Delta<TResult> Select<T, TResult>(this Delta<T> delta, Func<T, TResult> map)
		{
			return new Delta<TResult>(map(delta.Original), map(delta.Changed), delta.ChangedFields);
		}

		public static Delta<TResult> Select<TResult>(this Delta delta, Func<object, TResult> map)
		{
			return new Delta<TResult>(map(delta.Original), map(delta.Changed), delta.ChangedFields);
		}
	}
}
