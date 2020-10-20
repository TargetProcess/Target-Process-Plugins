using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Tp.BusinessObjects;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Core.Extensions;

namespace Tp.Model.Common
{
    public static class EntityKindExtensionsLight
    {
        private static readonly Dictionary<string, EntityKind> Names;
        private static readonly Dictionary<EntityKind, string> NamesByEntityKind;
        private static readonly Dictionary<string, EntityKind> Descriptions;

        static EntityKindExtensionsLight()
        {
            Names = Enum.GetValues(typeof(EntityKind))
                .Cast<EntityKind>()
                .ToDictionary(x => x.ToString(), StringCaseInsensitiveComparer.Instance);
            NamesByEntityKind = Names.ToDictionary(x => x.Value, x => x.Key);
            Descriptions = typeof(EntityKind).GetFields()
                .ToLookup(x => Attribute.GetCustomAttribute(x, typeof(DescriptionAttribute)) as DescriptionAttribute)
                .Where(x => x.Key != null)
                .ToDictionary(x => x.Key.Description, x => Names[x.Single().Name], StringCaseInsensitiveComparer.Instance);
        }

        public static EntityKind? ToEntityKind(this string name)
        {
            if (name.IsNullOrWhitespace())
                return null;
            name = name.Replace(" ", "");
            name = name.Singularize();

            return Descriptions.GetValue(name).OrElse(() => Names.GetValue(name))
                .OrElse(() => ExtendableDomainEntityKindExtensionsLight.Instance.EntityKinds.GetValue(name)).ToNullable();
        }

        [CanBeNull]
        public static string ToEntityKindName(this EntityKind entityKind)
        {
            return NamesByEntityKind.GetValue(entityKind)
                .OrElse(() => ExtendableDomainEntityKindExtensionsLight.Instance.EntityKindNames.GetValue(entityKind)).GetOrDefault();
        }
    }
}
