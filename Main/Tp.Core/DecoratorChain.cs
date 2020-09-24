using System;
using System.Collections.Generic;
using Tp.Core.Annotations;

namespace Tp.Core
{
    public class DecoratorChain<T>
    {
        private readonly object _sync = new object();

        [CanBeNull]
        private List<Func<T, T>> _builders;

        [CanBeNull]
        private Func<T, T> _firstBuilder;

        public void Decorate(Func<T, T> f)
        {
            lock (_sync)
            {
                if (_builders != null)
                {
                    _builders.Add(f);
                }
                else if (_firstBuilder != null)
                {
                    _builders = new List<Func<T, T>>(2);
                    _builders.Add(_firstBuilder);
                    _builders.Add(f);
                    _firstBuilder = null;
                }
                else
                {
                    _firstBuilder = f;
                }
            }
        }

        public T Get(T initial)
        {
            lock (_sync)
            {
                if (_builders != null)
                {
                    var current = initial;
                    foreach (var builder in _builders)
                    {
                        current = builder(current);
                    }

                    return current;
                }

                if (_firstBuilder != null)
                {
                    return _firstBuilder(initial);
                }

                return initial;
            }

        }
    }
}
