using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Tp.Core.Diagnostics
{
	public abstract class PerformanceCounters : IPerformanceCounters
	{
		private readonly IPerformanceCountersCategoryInfo _category;
		private readonly IDictionary<string, IPerformanceCounter> _counters;
		private readonly ILog _log;

		protected PerformanceCounters(IPerformanceCountersCategoryInfo category)
		{
			_category = category;
			_log = TpLogManager.Instance.PerformanceCounterLog();
			_counters = _category.CreationData.ToDictionary(c => c.CounterName,
				c => GetPerformanceCounter(_category.CategoryName, c.CounterName, false));
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

		private IPerformanceCounter GetPerformanceCounter(string categoryName, string counterName, bool isReadOnly)
		{
			try
			{
				var performanceCounter = new System.Diagnostics.PerformanceCounter(categoryName, counterName, isReadOnly);
				return new PerformanceCounter(performanceCounter, _log);
			}
			catch (Exception ex)
			{
				_log.Error("Error occured during getting performance counter: category = {0}, counter = {1}".Fmt(categoryName, counterName), ex);
				return new NullSafePerformanceCounter();
			}
		}
	}
}
