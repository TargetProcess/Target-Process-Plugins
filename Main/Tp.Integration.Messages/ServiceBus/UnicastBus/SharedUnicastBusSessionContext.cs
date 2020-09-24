using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tp.Core.Extensions;

namespace Tp.Integration.Messages.ServiceBus.UnicastBus
{
    public class SharedUnicastBusSessionContext
    {
        private readonly ConcurrentDictionary<string, object> _values;

        public SharedUnicastBusSessionContext() : this(new ConcurrentDictionary<string, object>())
        {
        }

        public SharedUnicastBusSessionContext(ConcurrentDictionary<string, object> values) => _values = values;

        public bool TryGet<T>(string key, out T value)
        {
            var hasValue = _values.TryGetValue(key, out var objectValue);
            value = hasValue ? (T) objectValue : default;
            return hasValue;
        }

        public T GetOrAdd<T>(string key, Func<string, T> valueFactory) => (T) _values.GetOrAdd(key, k => valueFactory(k));

        public void Set<T>(string key, T value) => _values[key] = value;

        public bool Remove<T>(string key, out T value)
        {
            var hasValue = _values.TryRemove(key, out var objectValue);
            value = hasValue ? (T) objectValue : default(T);
            return hasValue;
        }

        public SharedUnicastBusSessionContext Clone()
        {
            var values = new ConcurrentDictionary<string, object>();
            foreach (var (key, value) in _values)
            {
                values.AddOrThrow(key, value, e => e);
            }
            return new SharedUnicastBusSessionContext(values);
        }
    }
}
