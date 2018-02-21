using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
    public class NoProfilerDataCollector : IProfilerDataCollector
    {
        public static readonly NoProfilerDataCollector Instance = new NoProfilerDataCollector();

        private NoProfilerDataCollector()
        {
        }

        public void AddData(IEnumerable<TimeInterval> data)
        {
        }
    }
}
