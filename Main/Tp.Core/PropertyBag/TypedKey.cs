using System;

namespace Tp.Core.PropertyBag
{
    public class TypedKey<T> : TypedKey
    {
        public TypedKey(string name = "") : base(typeof(T), name)
        {
        }
    }

    public class TypedKey
    {
        public TypedKey(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public Type Type { get; }
        public string Name { get; }
    }
}
