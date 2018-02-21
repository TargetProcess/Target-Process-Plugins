namespace Tp.Core.Diagnostics.Counters
{
    public class DiagnosticCounterData
    {
        public DiagnosticCounterData(CounterKey counterKey, ulong value)
        {
            CounterKey = counterKey;
            Value = value;
        }

        public CounterKey CounterKey {get; }
        public ulong Value { get; }
    }
}
