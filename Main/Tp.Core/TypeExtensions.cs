using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tp.Core;
using Tp.Core.Annotations;

namespace System
{
    public static class TypeExtensions
    {
        public static object DefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
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

        public static Type GetCommonBaseType(this IEnumerable<Type> types)
        {
            return types.Aggregate((type1, type2) =>
            {
                for (var type = type2; type != null; type = type.BaseType)
                {
                    if (type.IsAssignableFrom(type1))
                    {
                        return type;
                    }
                }
                throw new InvalidCastException("Can not find base type");
            });
        }

        public static Maybe<Type> GetCommonBaseInterfaceType(this IEnumerable<Type> types)
        {
            return
                types.Select(t => t.GetInterfaces().Concat(t))
                    .Aggregate((a, b) => a.Intersect(b))
                    .OrderBy(x => x, Comparer<Type>.Create((a, b) =>
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
                    }))
                    .FirstOrNothing();
        }

        public static bool IsSameTypeOrNullable(this Type target, Type type)
        {
            return target == type || Nullable.GetUnderlyingType(target) == type;
        }

        /// <summary>
        /// Gets <typeparamref name="TInterface"/> method <paramref name="methodName"/> implementation from type <paramref name="implementationType"/>.
        /// </summary>
        /// <param name="implementationType">Type which implements <typeparamref name="TInterface"/>.</param>
        /// <param name="methodName">Name of the method to get implementation of.</param>
        /// <typeparam name="TInterface">Interface which </typeparam>
        /// <returns><typeparamref name="TInterface"/> method <paramref name="methodName"/> implementation from type <paramref name="implementationType"/>.</returns>
        /// <exception cref="MissingMethodException">
        /// Method <paramref name="methodName"/> is missing from interface <typeparamref name="TInterface"/>
        /// -or-
        /// Method <paramref name="methodName"/> is missing from implementation of interface <typeparamref name="TInterface"/> by type <paramref name="implementationType"/>.
        /// </exception>
        [NotNull]
        public static MethodInfo GetImplementationMethodOf<TInterface>(this Type implementationType, string methodName)
        {
            return implementationType.GetImplementationMethodOf(typeof(TInterface), methodName);
        }

        [NotNull]
        internal static MethodInfo GetImplementationMethodOf(this Type implementationType, Type interfaceType, string methodName)
        {
            var interfaceMethod = interfaceType.GetMethod(methodName);
            if (interfaceMethod == null)
            {
                throw new MissingMethodException($"Method {methodName} is missing from interface {interfaceType}");
            }
            var interfaceMapping = implementationType.GetInterfaceMap(interfaceType);
            for (int i = 0; i < interfaceMapping.InterfaceMethods.Length; i++)
            {
                if (interfaceMapping.InterfaceMethods[i].Equals(interfaceMethod))
                {
                    return interfaceMapping.TargetMethods[i];
                }
            }
            throw new MissingMethodException($"Method {interfaceMethod} is missing from interface {interfaceType} mapping of {implementationType}");
        }
    }
}
