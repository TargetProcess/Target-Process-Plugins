using Tp.BusinessObjects;
using Tp.Core.Annotations;

namespace Tp.Model.Common
{
    public static class ExtendableDomain
    {
        public static readonly int ExtendableDomainEntityTypeIdThreshold = 9000;

        [Pure]
        public static bool IsExtendableDomainType(int entityTypeId) => entityTypeId >= ExtendableDomainEntityTypeIdThreshold;

        [Pure]
        public static bool IsExtendableDomainKind(EntityKind kind) => (int) kind >= ExtendableDomainEntityTypeIdThreshold;
    }
}
