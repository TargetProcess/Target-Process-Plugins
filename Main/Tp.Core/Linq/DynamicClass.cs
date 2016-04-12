using System.Reflection;
using System.Text;

namespace System.Linq.Dynamic
{
	public abstract class DynamicClass
	{
		private readonly PropertyInfo[] _props;

		protected DynamicClass()
		{
			_props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("{");
			for (int i = 0; i < _props.Length; i++)
			{
				if (i > 0) sb.Append(", ");
				sb.Append(_props[i].Name);
				sb.Append("=");
				sb.Append(_props[i].GetValue(this, null));
			}
			sb.Append("}");
			return sb.ToString();
		}
	}
}
