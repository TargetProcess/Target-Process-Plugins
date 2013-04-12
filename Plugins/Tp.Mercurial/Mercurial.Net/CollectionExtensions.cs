using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mercurial
{
    /// <summary>
    /// This class contains extension methods for <see cref="Collection{T}"/>.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds all elements from the <paramref name="source"/> into <paramref name="target"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements to add.
        /// </typeparam>
        /// <param name="target">
        /// The collection to add the elements to.
        /// </param>
        /// <param name="source">
        /// The collection to add elements from.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="target"/> is <c>null</c>.</para>
        /// </exception>
        public static void AddRange<T>(this Collection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (source != null)
                foreach (T element in source)
                    target.Add(element);
        }
    }
}