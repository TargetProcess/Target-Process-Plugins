// ReSharper disable CheckNamespace

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public partial class Reflect
{
	public static readonly MethodInfo SelectMethod = GetMethod(() => new[] { 1 }.Select(default(Func<int, int>))).GetGenericMethodDefinition();

	public static PropertyInfo GetIndexer<T, TIndexer>()
	{
		return (from propertyInfo in typeof(T).GetProperties()
				let parameterInfos = propertyInfo.GetIndexParameters()
				where parameterInfos.Length > 0 && parameterInfos[0].ParameterType == typeof(TIndexer)
				select propertyInfo).FirstOrDefault();
	}
}
// ReSharper restore CheckNamespace
