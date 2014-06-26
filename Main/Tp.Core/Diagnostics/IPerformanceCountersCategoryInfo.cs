using System.Collections.Generic;
using System.Diagnostics;

namespace Tp.Core.Diagnostics
{
	public interface IPerformanceCountersCategoryInfo
	{
		string CategoryName { get; }
		string CategoryDescription { get; }
		IEnumerable<CounterCreationData> CreationData { get; }
	}
}