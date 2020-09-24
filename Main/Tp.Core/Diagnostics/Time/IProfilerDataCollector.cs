using System.Collections.Generic;
using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Time
{
    public interface IProfilerDataCollector
    {
        void AddData([NotNull] IEnumerable<TimeInterval> data);
    }
}
