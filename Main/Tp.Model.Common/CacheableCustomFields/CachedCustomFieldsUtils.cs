using System.Collections.Generic;
using StructureMap;

namespace Tp.Model.Common.CacheableCustomFields
{
    public static class CachedCustomFieldsUtils
    {
        public static IReadOnlyList<CacheableCustomField> GetCachedCustomFields()
        {
            return ObjectFactory.GetInstance<ICachedCustomFieldProvider>().Get().Fields;
        }
    }
}
