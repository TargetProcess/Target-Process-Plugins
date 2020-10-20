using Tp.Core.Annotations;

namespace Tp.Model.Common.CacheableCustomFields
{
    public interface ICachedCustomFieldProvider
    {
        [NotNull]
        CacheableCustomFieldSchema Get();
    }
}
