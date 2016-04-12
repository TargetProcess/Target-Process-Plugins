using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;

namespace Tp.Core.Linq
{
	public class SpecificExtensionMethodsProvider : IMethodProvider
	{
		private readonly Type _type;
		private readonly Func<MethodInfo, bool> _filter;

		public SpecificExtensionMethodsProvider(Type type, Func<MethodInfo, bool> filter)
		{
			_type = type;
			_filter = filter;
		}

		public IEnumerable<MethodInfo> GetExtensionMethodInfo()
		{
			return new AllExtensionMethodsProvider(_type)
				.GetExtensionMethodInfo()
				.Where(_filter);
		}
	}
}
