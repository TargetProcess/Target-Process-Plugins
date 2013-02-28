using System;

namespace Tp.Core.PropertyBag
{
	public class TypedKey<T> : TypedKey
	{
		public TypedKey() : base(typeof(T))
		{
		}
	}

	public class TypedKey
	{
		private readonly Type _type;
		public TypedKey(Type type)
		{
			_type = type;
		}

		public Type Type
		{
			get { return _type; }
		}
	}
}