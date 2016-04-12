using System;
using System.Reflection;
using Tp.Core;

namespace Tp.Utils.Documentation
{
	public static class DocumentationExtensions
	{
		public static string GetApiDescription(this ICustomAttributeProvider provider)
		{
			return provider.GetCustomAttribute<ApiDescriptionAttribute>().Select(a => a.Description).GetOrDefault(String.Empty);
		}
	}
}
