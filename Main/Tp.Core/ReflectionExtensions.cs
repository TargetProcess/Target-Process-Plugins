using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tp.Core;

namespace System
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// When overridden in a derived class, returns the <see cref="propertyInfo"/> object for the
        /// method on the direct or indirect base class in which the property represented
        /// by this instance was first declared.
        /// </summary>
        /// <returns>A <see cref="propertyInfo"/> object for the first implementation of this property.</returns>
        public static PropertyInfo GetBaseDefinition(this PropertyInfo propertyInfo)
        {
            var method = propertyInfo.GetAccessors(true)[0];
            if (method == null)
                return null;

            var baseMethod = method.GetBaseDefinition();

            if (baseMethod == method && baseMethod.DeclaringType == baseMethod.ReflectedType)
                return propertyInfo;

            var allProperties = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            var arguments = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();

            return baseMethod.DeclaringType.GetProperty(propertyInfo.Name, allProperties, null, propertyInfo.PropertyType, arguments, null);
        }

        public static bool IsExtensionMethod(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<ExtensionAttribute>().HasValue;
        }

        public static Maybe<PropertyInfo> FindPropertyInHierarchy(this Type type, string name,
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance)
        {
            return type.GetProperty(name, flags)
                .NothingIfNull()
                .OrElse(() => type.BaseType
                    .NothingIfNull()
                    .Bind(t => t.FindPropertyInHierarchy(name, flags)));
        }

        public static readonly Memoizator<(ICustomAttributeProvider, Type), Maybe<Attribute>> CustomAttributesMemo =
            new Memoizator<(ICustomAttributeProvider, Type), Maybe<Attribute>>(x =>
            {
                var (provider, attributeType) = x;
                return provider.GetCustomAttributes(attributeType, false).OfType<Attribute>().FirstOrNothing();
            });

        public static Maybe<TAttribute> GetCustomAttributeCached<TAttribute>(
            this ICustomAttributeProvider provider)
            where TAttribute : Attribute
        {
            return CustomAttributesMemo.Apply((provider, typeof(TAttribute))).OfType<TAttribute>();
        }
    }
}
