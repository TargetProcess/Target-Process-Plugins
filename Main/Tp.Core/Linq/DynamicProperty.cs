namespace System.Linq.Dynamic
{
	public class DynamicProperty
	{
		private readonly string _name;
		private readonly Type _type;

		public DynamicProperty(string name, Type type)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (type == null) throw new ArgumentNullException("type");
			_name = name;
			_type = type;
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