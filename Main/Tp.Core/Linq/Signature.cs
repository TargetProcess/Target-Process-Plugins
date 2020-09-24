using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic
{
    internal class Signature : IEquatable<Signature>
    {
        public readonly Type BaseType;
        private readonly int _hashCode;
        public readonly DynamicProperty[] Properties;

        public Signature(IEnumerable<DynamicProperty> properties, Type baseType)
        {
            BaseType = baseType ?? typeof(object);
            Properties = properties.ToArray();
            _hashCode = BaseType.GetHashCode();
            foreach (var p in Properties)
            {
                _hashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

        public bool Equals(Signature other)
        {
            if (Properties.Length != other.Properties.Length) return false;
            if (BaseType != other.BaseType) return false;
            return !Properties.Where((t, i) => t.Name != other.Properties[i].Name || t.Type != other.Properties[i].Type).Any();
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj) => obj is Signature signature && Equals(signature);
    }
}
