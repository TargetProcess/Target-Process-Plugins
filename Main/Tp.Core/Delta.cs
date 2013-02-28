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

		public static Delta<T> Create<T>(T original, T changed, IEnumerable<string> changedFields = null)
		{
			return new Delta<T>(original, changed, changedFields ?? Enumerable.Empty<string>());
		}
	}

	public class Delta<TData> : Delta
	{
		internal Delta(TData original, TData changed, IEnumerable<string> changedFields) : base(original, changed, changedFields){}

		public new TData Original
		{
			get { return (TData) base.Original; }
		}

		public new TData Changed
		{
			get { return (TData) base.Changed; }
		}
	}

	public static class DeltaExtensions
	{
		public static string Dump<T>(this IEnumerable<Delta<T>> deltas)
		{
			return string.Join(Environment.NewLine, deltas.Select(x => x.ToString()));
		}
	}
}
