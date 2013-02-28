using System.Data;

namespace System
{
	[AttributeUsage(AttributeTargets.Method)]
	public class SqlFunctionAttribute : Attribute
	{
		public string Name { get; private set; }

		public DbType DbType { get; private set; }

		public SqlFunctionAttribute(string name, DbType dbType)
		{
			Name = name;
			DbType = dbType;
		}
	}
}