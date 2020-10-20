using Tp.BusinessObjects;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Integration.Common;

namespace Tp.Model.Common.Entities.EntityType
{
    /// <summary>
    /// Represents immutable information about EntityType which can safely be cached and reused in-memory
    /// between different accounts and sessions.
    /// </summary>
    public class CacheableEntityType
    {
        public CacheableEntityType(
            int entityTypeId,
            [NotNull] string name,
            EntityKind entityKind,
            CustomFieldScopeEnum customFieldScope,
            bool isUnitInHourOnly)
        {
            EntityTypeId = entityTypeId;
            Name = Argument.NotNull(nameof(name), name);
            EntityKind = entityKind;
            CustomFieldScope = customFieldScope;
            IsUnitInHourOnly = isUnitInHourOnly;
        }

        /// <summary>
        /// For static types, it is Abbreviation
        /// </summary>
        public string Name { get; }

        public int EntityTypeId { get; }

        public EntityKind EntityKind { get; }

        public CustomFieldScopeEnum CustomFieldScope { get; }

        public bool IsUnitInHourOnly { get; }

        public string GetHumanReadableName() => Name.Humanize();

        public string GetHumanReadableNamePlural() => GetHumanReadableName().Pluralize();
    }
}
