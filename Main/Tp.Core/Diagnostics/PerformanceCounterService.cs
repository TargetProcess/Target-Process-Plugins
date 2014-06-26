using System.Collections.Generic;
using StructureMap;
using Tp.Core.Services;

namespace Tp.Core.Diagnostics
{
	public class PerformanceCounterService : IService
	{
		private readonly IContainer _container;
		private readonly IEnumerable<IPerformanceCounters> _performanceCounters;

		public PerformanceCounterService(IContainer container)
		{
			_container = container;
			_performanceCounters = _container.GetAllInstances<IPerformanceCounters>();
		}

		public void Start()
		{
		}

		public void Stop()
		{
			foreach (var counters in _performanceCounters)
			{
				counters.Dispose();
			}
		}
	}
}