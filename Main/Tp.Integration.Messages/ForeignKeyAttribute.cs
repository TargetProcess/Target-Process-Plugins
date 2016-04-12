using System;

namespace Tp.Integration.Common
{
	/// <summary>
	/// It is marker for foreign key attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	[Serializable]
	public class ForeignKeyAttribute : Attribute
	{
	}
}
