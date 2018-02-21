using System.Collections.Generic;
using Tp.Core.Annotations;

namespace Tp.Core
{
    public static class CollectionExtensions
    {
        public static int AddRange<T>(
            [NotNull] this HashSet<T> hashSet, 
            [NotNull] IEnumerable<T> items)
        {
            var addedCount = 0;

            foreach (var item in items)
            {
                if (hashSet.Add(item))
                {
                    addedCount++;
                }
            }

            return addedCount;
        }
    }
}
