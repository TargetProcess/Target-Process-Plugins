using System;

namespace Tp.Testing.Common.NBehave
{
	/// <summary>
	/// Assembly which contains action steps should be marked with this attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ActionStepsAssemblyAttribute : Attribute
	{
	}
}
