// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Linq;

namespace System.Collections.Generic
{
	public static class LazyAggregatorExtensions
	{
		public static LazyAggregator<T> LazyLastOrDefault<TRecord, T>(this IEnumerable<TRecord> records,
		                                                              Func<TRecord, T> selector)
		{
			var enumerable = records.Select(selector);
			return new LazyAggregator<T>(enumerable, items => items.LastOrDefault(x => !Equals(x, null)));
		}
	}
}