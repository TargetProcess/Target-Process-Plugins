using System.Collections.Generic;

namespace System.Linq.Dynamic
{
	internal class Signature : IEquatable<Signature>
	{
		public readonly int _hashCode;
		public DynamicProperty[] _properties;

		public Signature(IEnumerable<DynamicProperty> properties)
		{
			_properties = properties.ToArray();
			_hashCode = 0;
			foreach (DynamicProperty p in _properties)
			{
				_hashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
			}
		}

		#region IEquatable<Signature> Members

		public bool Equals(Signature other)
		{
			if (_properties.Length != other._properties.Length) return false;
			return !_properties.Where((t, i) => t.Name != other._properties[i].Name || t.Type != other._properties[i].Type).Any();
		}

		#endregion

		public override int GetHashCode()
		{
			return _hashCode;
		}

		public override bool Equals(object obj)
		{
			return obj is Signature && Equals((Signature) obj);
		}
	}
}