using System.Collections.Generic;
using System.Linq;

namespace Tp.Core.Diagnostics.Time
{
	public abstract class TimePointsBase : ITimePoints
	{
		public abstract IEnumerable<TimePoint> Points { get; }
		public abstract void Add(TimePoint point);
		public abstract void AddUtcNow(string name);
		public abstract string Dump();
		public abstract ITimePoints CreateFork();

		public void AddRange(IEnumerable<TimePoint> points)
		{
			points.ForEach(Add);
		}
	}
}
