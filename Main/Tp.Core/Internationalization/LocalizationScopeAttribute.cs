using System;

namespace Tp.Core.Internationalization
{
	[AttributeUsage(AttributeTargets.Class)]
	public class LocalizationScopeAttribute : Attribute
	{
		public string Scope { get; private set; }

		public LocalizationScopeAttribute(string scope)
		{
			Scope = scope;
		}
	}
}
