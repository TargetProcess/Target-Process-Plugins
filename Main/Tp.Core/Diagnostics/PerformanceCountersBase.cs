using System.Collections.Generic;
using System.Linq;

namespace Tp.Core.Diagnostics
{
	public abstract class PerformanceCountersBase<T> : IPerformanceCounters
		where T : IPerformanceCountersCategoryInfo
	{
		private readonly T _category;
		private readonly IDictionary<string, IPerformanceCounter> _counters = new Dictionary<string, IPerformanceCounter>();

		protected PerformanceCountersBase(T category)
		{
			_category = category;
			foreach (var counterCreationData in _category.CreationData)
			{
				_counters.Add(counterCreationData.CounterName, GetPerformanceCounter(_category.CategoryName, counterCreationData.CounterName, false));
			}
		}

		private static IPerformanceCounter GetPerformanceCounter(string categoryName, string counterName, bool isReadOny)
		{
			try
			{
				return new PerformanceCounter(GetSourcePerformanceCounter(categoryName, counterName, isReadOny));
			}
			catch
			{
				return new NullSafePerformanceCounter();
			}
		}

		private static System.Diagnostics.PerformanceCounter GetSourcePerformanceCounter(string categoryName, string counterName, bool isReadOny)
		{
			return new System.Diagnostics.PerformanceCounter(categoryName, counterName, isReadOny);
		}

		protected IPerformanceCounter this[string counterName]
		{
			get { return _counters.GetOrAdd(counterName, name => GetPerformanceCounter(_category.CategoryName, counterName, false)); }
		}

		public void Dispose()
		{
			foreach (var performanceCounter in _counters.Values.Where(x => x != null))
			{
				performanceCounter.Dispose();
			}
		}
	}
}