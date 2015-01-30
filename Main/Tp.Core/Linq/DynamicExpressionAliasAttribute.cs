namespace System.Linq.Dynamic
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	public class DynamicExpressionAliasAttribute : Attribute
	{
		public string Name { get; set; }

		public DynamicExpressionAliasAttribute(string name)
		{
			Name = name;
		}
	}
}