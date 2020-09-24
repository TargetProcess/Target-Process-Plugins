using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tp.Core
{
    public sealed class ConcurrentHashSet<T> : ISet<T>
    {
        private readonly object _monitor = new object();
        private readonly HashSet<T> _set;

        public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> instance)
        {
            _set = new HashSet<T>(collection, instance);
        }

        public ConcurrentHashSet(IEqualityComparer<T> instance)
        {
            _set = new HashSet<T>(instance);
        }

        public void Clear()
        {
            lock (_monitor)
            {
                _set.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_monitor)
            {
                return _set.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_monitor)
            {
                _set.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (_monitor)
            {
                return _set.Remove(item);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            lock (_monitor)
            {
                _set.GetObjectData(info, context);
            }
        }

        public void OnDeserialization(object sender)
        {
            lock (_monitor)
            {
                _set.OnDeserialization(sender);
            }
        }

        public bool Add(T item)
        {
            lock (_monitor)
            {
                return _set.Add(item);
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                _set.UnionWith(other);
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                _set.IntersectWith(other);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                _set.ExceptWith(other);
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                _set.SymmetricExceptWith(other);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                return _set.IsSubsetOf(other);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                return _set.IsProperSubsetOf(other);
            }
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                return _set.IsSupersetOf(other);
            }
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                return _set.IsProperSupersetOf(other);
            }
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                return _set.Overlaps(other);
            }
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            lock (_monitor)
            {
                return _set.SetEquals(other);
            }
        }

        public void CopyTo(T[] array)
        {
            lock (_monitor)
            {
                _set.CopyTo(array);
            }
        }

        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            lock (_monitor)
            {
                _set.CopyTo(array, arrayIndex, count);
            }
        }

        public int RemoveWhere(Predicate<T> match)
        {
            lock (_monitor)
            {
                return _set.RemoveWhere(match);
            }
        }

        public void TrimExcess()
        {
            lock (_monitor)
            {
                _set.TrimExcess();
            }
        }

        public int Count
        {
            get
            {
                lock (_monitor)
                {
                    return _set.Count;
                }
            }
        }

        public IEqualityComparer<T> Comparer
        {
            get
            {
                lock (_monitor)
                {
                    return _set.Comparer;
                }
            }
        }

        void ICollection<T>.Add(T item)
        {
            lock (_monitor)
            {
                _set.Add(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_monitor)
            {
                return _set.GetEnumerator();
            }
        }

        public bool IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
