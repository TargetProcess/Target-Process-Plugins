using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Core.Diagnostics.Counters
{
    class DiagnosticCountersService : IDiagnosticCountersService
    {
        private readonly ConcurrentDictionary<string, DiagnosticCounter> _counters = new ConcurrentDictionary<string, DiagnosticCounter>();

        public IDiagnosticCounter GetOrCreateCounter(CounterKey counterKey)
        {
            return _counters.GetOrAdd(counterKey.Value, _ => new DiagnosticCounter(counterKey));
        }

        public IReadOnlyList<DiagnosticCounterData> GetData(CounterKey counterKey)
        {
            return _counters
                .Where(pair => pair.Key.StartsWith(counterKey.Value))
                .Select(pair => pair.Value.GetData())
                .ToReadOnlyCollection();
        }

        private class DiagnosticCounter : IDiagnosticCounter
        {
            private readonly CounterKey _key;
            private ulong _value;

            public DiagnosticCounter(CounterKey key)
            {
                _key = key;
            }

            public void Increment(ulong val)
            {
                try
                {
                    checked
                    {
                        _value += val;
                    }
                }
                catch (OverflowException)
                {
                    _value = 0;
                }
            }

            public void Decrement(ulong val)
            {

                try
                {
                    checked
                    {
                        _value -= val;
                    }
                }
                catch (OverflowException)
                {
                    _value = 0;
                }
            }

            public DiagnosticCounterData GetData()
            {
                return new DiagnosticCounterData(_key, _value);
            }
        }
    }
}
