using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
    public interface IProfilerDataCollector
    {
        void AddData(IEnumerable<TimeInterval> data);
    }
}
