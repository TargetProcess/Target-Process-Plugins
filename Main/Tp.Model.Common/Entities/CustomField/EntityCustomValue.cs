using System;
using Tp.BusinessObjects;

namespace Tp.Model.Common.Entities.CustomField
{
    public class EntityCustomValue : IEquatable<EntityCustomValue>
    {
        public int? Id { get; set; }
        public EntityKind EntityKind { get; set; }
        public string Name { get; set; }

        public bool Equals(EntityCustomValue other)
        {
            return other != null && Id == other.Id && EntityKind == other.EntityKind && Name == other.Name;
        }

        public override bool Equals(object obj) => Equals(obj as EntityCustomValue);

        public override int GetHashCode() => Id == null ? 0 : Id.Value.GetHashCode();

        public override string ToString() => $"{EntityKind}#{Id} {Name}";
    }
}
