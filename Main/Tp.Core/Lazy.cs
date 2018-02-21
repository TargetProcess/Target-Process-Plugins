using System.Threading;

namespace System
// ReSharper restore CheckNamespace
{
    public static class Lazy
    {
        public static Lazy<T> Create<T>(Func<T> valueFactory,
            LazyThreadSafetyMode threadSafetyMode = LazyThreadSafetyMode.ExecutionAndPublication)
        {
            return new Lazy<T>(valueFactory, threadSafetyMode);
        }

        public static Lazy<T> Create<T>(T value, LazyThreadSafetyMode threadSafetyMode = LazyThreadSafetyMode.ExecutionAndPublication)
        {
            return new Lazy<T>(() => value, threadSafetyMode);
        }

        /// <summary>
        /// Lazy instance which retries to create value if previous attempt raised error
        /// I decide not to use System.Threading.Lazy class with parameter LazyThreadSafetyMode.PublicationOnly with similar exception semantics because 
        /// LazyThreadSafetyMode.PublicationOnly allows to create several instances of value if several threads attempts to read value first time - resource usage bla-bla-bla
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static TryCreateValueOnEveryFailedUnboxLazy<T> CreateLazyTryingToCreateValueOnEveryFailedUnbox<T>(Func<T> valueFactory)
        {
            return new TryCreateValueOnEveryFailedUnboxLazy<T>(valueFactory);
        }
    }

    public class TryCreateValueOnEveryFailedUnboxLazy<T>
    {
        private readonly Func<T> _valueFactory;
        private bool _initialized;
        private object _gate;
        private T _value;

        public TryCreateValueOnEveryFailedUnboxLazy(Func<T> valueFactory)
        {
            _valueFactory = valueFactory;
            _initialized = false;
            _gate = new object();
            _value = default;
        }

        public T Value
        {
            get
            {
                EnsureCreated();
                return _value;
            }
        }

        public void EnsureCreated()
        {
            LazyInitializer.EnsureInitialized(ref _value, ref _initialized, ref _gate, _valueFactory);
        }
    }
}
