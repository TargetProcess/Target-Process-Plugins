using System;
using System.Collections;

namespace Tp.Components.Extensions
{
    public static class CollectionUtils
    {
        public static IEnumerable GetPage(this IEnumerable list, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("pageNumber and pageSize must be greater than zero");
            }

            var startIndex = (pageNumber - 1) * pageSize;
            var endIndex = startIndex + pageSize - 1;

            var index = 0;
            foreach (var item in list)
            {
                if (startIndex <= index && index <= endIndex)
                {
                    yield return item;
                }
                if (index > endIndex)
                {
                    break;
                }
                index++;
            }
        }
    }
}
