using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tp.Core
{
    public class HierarchyStructureNode : IEquatable<HierarchyStructureNode>
    {
        public string Id { get; private set; }
        public ReadOnlyCollection<HierarchyStructureNode> Nodes { get; private set; }

        public HierarchyStructureNode(string id, HierarchyStructureNode[] nodes = null)
        {
            Id = id;
            Nodes = new ReadOnlyCollection<HierarchyStructureNode>(nodes ?? Array.Empty<HierarchyStructureNode>());
        }

        public bool Equals(HierarchyStructureNode other)
        {
            return Id == other.Id && Nodes.SequenceEqual(other.Nodes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(HierarchyStructureNode)) return false;
            return Equals((HierarchyStructureNode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Nodes.Aggregate(0, (acc, x) => acc ^ x.GetHashCode()) * 397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }
    }
}
