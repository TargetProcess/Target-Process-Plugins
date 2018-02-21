using System.Collections.Generic;
using System.Linq;

namespace Tp.Core
{
    public static class CountsExtensions
    {
        public static int Sum<TKey>(this IEnumerable<GroupWithCount<TKey>> source)
        {
            return source.Sum(x => x.Count);
        }
    }
}
