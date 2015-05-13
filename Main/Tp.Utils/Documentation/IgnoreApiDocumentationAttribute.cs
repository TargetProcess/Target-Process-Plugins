using System;

namespace Tp.Utils.Documentation
{
	/// <summary>
	/// Declares that the specified controller action should not be mention in API documentation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
	public class IgnoreApiDocumentationAttribute : Attribute
	{
	}
}
