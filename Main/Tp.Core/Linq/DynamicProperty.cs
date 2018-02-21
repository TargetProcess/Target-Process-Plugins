namespace System.Linq.Dynamic
{
    public class DynamicProperty
    {
        private readonly string _name;
        private readonly Type _type;

        public DynamicProperty(string name, Type type)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));
            _name = name;
            _type = type;
        }

        protected bool Equals(DynamicProperty other)
        {
            return string.Equals(_name, other._name) && _type == other._type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DynamicProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_name != null ? _name.GetHashCode() : 0) * 397) ^ (_type != null ? _type.GetHashCode() : 0);
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}
