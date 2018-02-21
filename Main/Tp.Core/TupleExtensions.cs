using System;

namespace Tp.Core
{
    public static class TupleExtensions
    {
        public static void Decompose<T1, T2>(this Tuple<T1, T2> tuple, out T1 first, out T2 second)
        {
            first = tuple.Item1;
            second = tuple.Item2;
        }

        public static void Decompose<T1, T2, T3>(this Tuple<T1, T2, T3> tuple, out T1 first, out T2 second, out T3 third)
        {
            first = tuple.Item1;
            second = tuple.Item2;
            third = tuple.Item3;
        }
    }
}
