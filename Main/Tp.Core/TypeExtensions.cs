using System.Reflection;
using Tp.Core;

namespace System
{
	public static class TypeExtensions
	{
		public static object DefaultValue(this Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}

		public static Type ResultType(this MemberInfo memberInfo)
		{
			var resultType = memberInfo.MaybeAs<PropertyInfo>().Select(x => x.PropertyType)
			                           .OrElse(() => memberInfo.MaybeAs<MethodInfo>().Select(x => x.ReturnType))
			                           .OrElse(() => memberInfo.MaybeAs<FieldInfo>().Select(x => x.FieldType)).Value;
			return resultType;
		}
	}
}