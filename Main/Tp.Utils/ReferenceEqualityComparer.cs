using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Tp.Utils
{
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public static readonly ReferenceEqualityComparer<T> Default = new ReferenceEqualityComparer<T>();

        private ReferenceEqualityComparer()
        {
        }

        public bool Equals(T x, T y) =>
            ReferenceEquals(x, y);

        public int GetHashCode(T obj) =>
            RuntimeHelpers.GetHashCode(obj);
    }
}
