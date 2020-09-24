using System;
using System.Collections.Concurrent;

namespace Tp.Core
{
    public static class Memoize
    {
        public static Func<TArg, TResult> Create<TArg, TResult>(Func<TArg, TResult> f)
        {
            var m = new Memoizator<TArg, TResult>(f);
            return m.Apply;
        }
    }

    public class Memoizator<TArg, TResult>
    {
        private readonly ConcurrentDictionary<TArg, TResult> _data;
        private readonly Func<TArg, TResult> _func;

        public int Size => _data.Count;

        public Memoizator(Func<TArg, TResult> func)
        {
            _data = new ConcurrentDictionary<TArg, TResult>();
            _func = func;
        }

        public TResult Apply(TArg arg)
        {
            return _data.GetOrAdd(arg, x => _func(x));
        }
    }
}
