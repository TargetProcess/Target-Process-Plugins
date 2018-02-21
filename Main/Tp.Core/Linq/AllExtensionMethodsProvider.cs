using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Core.Features;

// ReSharper disable once CheckNamespace

namespace System.Linq.Dynamic
{
    public class AllExtensionMethodsProvider : IMethodProvider
    {
        private readonly Type _type;

        public AllExtensionMethodsProvider([NotNull] Type type)
        {
            _type = Argument.NotNull(nameof(type), type);
        }

        public IEnumerable<MethodInfo> GetExtensionMethodInfo()
        {
            var extensionMethodInfo = _type
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(x => x.GetCustomAttribute<ExtensionAttribute>().HasValue);
            return extensionMethodInfo;
        }
    }

    /// <summary>
    /// Marks the method as being available for usage in our public API relying on expression parsing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [MeansImplicitUse]
    public class PublicApiMethodAttribute : Attribute
    {
        public PublicApiMethodAttribute()
            : this(null)
        {
        }

        public PublicApiMethodAttribute(string description)
        {
            Description = description;
        }

        [CanBeNull]
        public string Description { get; }
    }

    public class ExplicitExtensionMethodsProvider : IMethodProvider
    {
        private readonly Type _type;

        public ExplicitExtensionMethodsProvider([NotNull] Type type)
        {
            _type = Argument.NotNull(nameof(type), type);
        }

        public IEnumerable<MethodInfo> GetExtensionMethodInfo()
        {
            var methods = _type
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(x => x.GetCustomAttribute<ExtensionAttribute>().HasValue);

            if (TpFeature.LimitPublicExtensionMethods.IsEnabled())
            {
                methods = methods.Where(x => x.GetCustomAttribute<PublicApiMethodAttribute>().HasValue);
            }

            return methods;
        }
    }
}
