using System;
using System.Collections.Generic;
using StructureMap;
using Tp.BusinessObjects;
using Tp.Core.Features;

namespace Tp.Model.Common
{
    public interface IExtendableDomainEntityKindExtensionsLight : IDisposable
    {
        IReadOnlyDictionary<string, EntityKind> EntityKinds { get; }

        IReadOnlyDictionary<EntityKind, string> EntityKindNames { get; }
    }

    public class ExtendableDomainEntityKindExtensionsLightSafeNull : IExtendableDomainEntityKindExtensionsLight
    {
        public static readonly ExtendableDomainEntityKindExtensionsLightSafeNull Instance =
            new ExtendableDomainEntityKindExtensionsLightSafeNull();

        private ExtendableDomainEntityKindExtensionsLightSafeNull()
        {
        }

        public IReadOnlyDictionary<string, EntityKind> EntityKinds { get; } = new Dictionary<string, EntityKind>(0);

        public IReadOnlyDictionary<EntityKind, string> EntityKindNames { get; } = new Dictionary<EntityKind, string>(0);

        public void Dispose()
        {
        }
    }

    public static class ExtendableDomainEntityKindExtensionsLight
    {
        public static IExtendableDomainEntityKindExtensionsLight Instance => TpFeature.ExtendableDomain.TryGetIsEnabled()
            ? ObjectFactory.TryGetInstance<IExtendableDomainEntityKindExtensionsLight>()
            ?? ExtendableDomainEntityKindExtensionsLightSafeNull.Instance
            : ExtendableDomainEntityKindExtensionsLightSafeNull.Instance;
    }
}
