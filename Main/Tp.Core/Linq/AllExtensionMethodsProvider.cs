using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Dynamic
{
	public class AllExtensionMethodsProvider : IMethodProvider
	{
		private readonly Type _type;

		public AllExtensionMethodsProvider(Type type)
		{
			_type = type;
		}

		public IEnumerable<MethodInfo> GetExtensionMethodInfo()
		{
			IEnumerable<MethodInfo> extensionMethodInfo = _type.GetMethods(BindingFlags.Public | BindingFlags.Static |
				BindingFlags.DeclaredOnly)
				.Where(x => x.GetCustomAttribute<ExtensionAttribute>().HasValue);
			return extensionMethodInfo;
		}
	}
}
