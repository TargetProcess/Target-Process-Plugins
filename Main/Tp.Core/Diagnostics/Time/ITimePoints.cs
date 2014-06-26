using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
	public interface ITimePoints
	{
		void Add(TimePoint point);
		void AddUtcNow(string name);
		string Dump();
		void CopyFrom(ITimePoints timePoints);
		ITimePoints CreateSnapshot();
		IEnumerable<TimePoint> Points { get; }
	}
}