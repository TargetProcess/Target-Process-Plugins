using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
    public class NoProfilerDataProcess : IProfilerDataProcessor
    {
        public void Process(IEnumerable<TimeInterval> data, string context)
        {
        }
    }
}
