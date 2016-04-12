using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Dynamic
{
	public interface IMethodProvider
	{
		IEnumerable<MethodInfo> GetExtensionMethodInfo();
	}
}
