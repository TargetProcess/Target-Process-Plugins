using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
	public interface ITimePoints : ITimePointsFork
	{
		void Add(TimePoint point);
		void AddUtcNow(string name);
		string Dump();
		IEnumerable<TimePoint> Points { get; }
	}
}