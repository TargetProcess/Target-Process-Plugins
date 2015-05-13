// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace System.Collections.Generic
{
	public class LazyAggregator<T>
	{
		private Func<T> _aggregator;


		public LazyAggregator(IEnumerable<T> values, Func<IEnumerable<T>, T> aggregator)
		{
			_aggregator = () =>
			          	{
			          		T value = aggregator(values);
			          		_aggregator = () => value;
			          		return value;
			          	};
		}

		public T Value
		{
			get { return _aggregator(); }
		}

		public static implicit operator T(LazyAggregator<T> aggregator)
		{
			return aggregator.Value;
		}
	}
}