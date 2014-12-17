using System.Collections.Generic;
using System.Linq;

namespace Tp.Core.Diagnostics
{
	public abstract class PerformanceCounters : IPerformanceCounters
	{
		private readonly IPerformanceCountersCategoryInfo _category;
		private readonly IDictionary<string, IPerformanceCounter> _counters;

		protected PerformanceCounters(IPerformanceCountersCategoryInfo category)
		{
			_category = category;
			_counters = _category.CreationData.ToDictionary(c => c.CounterName, c => GetPerformanceCounter(_category.CategoryName, c.CounterName, false));
		}

		public void Dispose()
		{
			foreach (var performanceCounter in _counters.Values.Where(x => x != null))
			{
				performanceCounter.Dispose();
			}
		}

		protected IPerformanceCounter this[string counterName]
		{
			get { return _counters.GetOrAdd(counterName, name => GetPerformanceCounter(_category.CategoryName, counterName, false)); }
		}

		private static IPerformanceCounter GetPerformanceCounter(string categoryName, string counterName, bool isReadOny)
		{
			try
			{
				var performanceCounter = new System.Diagnostics.PerformanceCounter(categoryName, counterName, isReadOny);
				return new PerformanceCounter(performanceCounter);
			}
			catch
			{
				return new NullSafePerformanceCounter();
			}
		}
	}
}