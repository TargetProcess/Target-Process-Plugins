using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
	public interface ITpProfilerDataCollector
	{
		void AddData(IEnumerable<TimeInterval> data);
	}
}