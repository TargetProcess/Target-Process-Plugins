using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Counters
{
    public interface IDiagnosticCountersService
    {
        IDiagnosticCounter GetOrCreateCounter(CounterKey counterKey);
        IReadOnlyList<DiagnosticCounterData> GetData(CounterKey counterKey);
    }
}
