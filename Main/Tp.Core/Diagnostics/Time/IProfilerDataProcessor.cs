using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
    public interface IProfilerDataProcessor
    {
        void Process(IEnumerable<TimeInterval> data, string context);
    }
}
