using System.Collections.Generic;
using System.Linq;
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

		public static IEnumerable<MemberInfo> GetInterfaceProperties(this Type type)
		{
			if (!type.IsInterface)
			{
				yield break;
			}

			var analyzedInterface = new List<Type>();
			var interfacesQueue = new Queue<Type>();
			analyzedInterface.Add(type);
			interfacesQueue.Enqueue(type);
			while (interfacesQueue.Count > 0)
			{
				var subType = interfacesQueue.Dequeue();
				foreach (var subInterface in
					subType.GetInterfaces().Where(subInterface => !analyzedInterface.Contains(subInterface)))
				{
					analyzedInterface.Add(subInterface);
					interfacesQueue.Enqueue(subInterface);
				}

				foreach (PropertyInfo propertyInfo in subType.GetProperties())
				{
					yield return propertyInfo;
				}
			}
		}

		/// <summary>
		/// Same as Type.GetMember but return members from base interfaces
		/// E.g. typeof(ICollection).GetMember("GetEnumerator") returns [] while typeof(ICollection).GetMemberInterfacewise("GetEnumerator")
		/// returns MemberInfo of IEnumerable.GetEnumerator
		/// </summary>
		public static MemberInfo[] GetMemberInterfacewise(this Type type, string name)
		{
			if (!type.IsInterface)
			{
				return type.GetMember(name);
			}
			return type.GetMember(name).Concat(type.GetInterfaces().SelectMany(i => i.GetMember(name))).ToArray();
		}

		public static Maybe<Type> GetCommonBaseInterfaceType(IEnumerable<Type> types)
		{
			return
				types.Select(t => t.GetInterfaces().Concat(t))
					.Aggregate((a, b) => a.Intersect(b))
					.OrderBy(x => x, LambdaComparer<Type>.Comparer(((a, b) =>
					{
						if (a != b)
						{
							if (a.IsAssignableFrom(b))
							{
								return 1;
							}
							if (b.IsAssignableFrom(a))
							{
								return -1;
							}
						}
						return 0;
					})))
					.FirstOrNothing();
		}
	}
}
