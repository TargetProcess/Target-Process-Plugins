using System;
using System.Collections;
using System.Collections.Generic;

namespace Tp.Core
{
    public static class ComparerHelper
    {
        public static IEqualityComparer<T> CombineAnd<T>(this IEqualityComparer<T> comparer1, IEqualityComparer<T> comparer2)
        {
            return LambdaComparer<T>.Equality((a, b) => comparer1.Equals(a, b) && comparer2.Equals(a, b),
                x => comparer1.GetHashCode(x) ^ comparer2.GetHashCode(x));
        }
    }

    public static class LambdaComparer<T>
    {
        public static IEqualityComparer<T> Equality<TKey>(Func<T, TKey> selector)
        {
            return new EqualityHelper<TKey>(selector);
        }

        public static IEqualityComparer<T> Equality(Func<T, T, bool> equality, Func<T, int> hash)
        {
            return new EqualityHelper2(equality, hash);
        }

        public static IComparer RawComparer(Func<T, T, int> f)
        {
            return new ComparerHelper(f);
        }

        private class ComparerHelper : IComparer
        {
            private readonly Func<T, T, int> _f;

            public ComparerHelper(Func<T, T, int> f)
            {
                _f = f;
            }

            public int Compare(object x, object y)
            {
                return _f((T) x, (T) y);
            }
        }

        private class EqualityHelper2 : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _equality;
            private readonly Func<T, int> _hash;

            public EqualityHelper2(Func<T, T, bool> equality, Func<T, int> hash)
            {
                _equality = equality;
                _hash = hash;
            }

            public bool Equals(T x, T y)
            {
                return _equality(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _hash(obj);
            }
        }


        private class EqualityHelper<TKey> : IEqualityComparer<T>
        {
            private readonly Func<T, TKey> _selector;

            public EqualityHelper(Func<T, TKey> selector)
            {
                _selector = selector;
            }

            public bool Equals(T x, T y)
            {
                return Equals(_selector(x), _selector(y));
            }

            public int GetHashCode(T obj)
            {
                var selector = _selector(obj);
                return Equals(selector, null) ? 0 : selector.GetHashCode();
            }
        }
    }
}
