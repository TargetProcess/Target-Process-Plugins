using System.Linq;
using System.Reflection;

public partial class Reflect
{
	public static PropertyInfo GetIndexer<T, TIndexer>()
	{
		return (from propertyInfo in typeof(T).GetProperties()
			let parameterInfos = propertyInfo.GetIndexParameters()
			where parameterInfos.Length > 0 && parameterInfos[0].ParameterType == typeof(TIndexer)
			select propertyInfo).FirstOrDefault();
	}
}

// ReSharper restore CheckNamespace
