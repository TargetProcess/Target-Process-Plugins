using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Core.Annotations;

namespace Tp.Core.Features
{
    public static class TpFeatureListExtensions
    {
        private static bool _tpFeatureListFactoryInitialized;
        private static ITpFeatureListFactory _tpFeatureListFactory;

        public static TpFeature[] GetEnabledFeatures(this ITpFeatureList list) =>
            Enum
                .GetValues(typeof(TpFeature))
                .Cast<TpFeature>()
                .Where(list.IsEnabled)
                .ToArray();

        public static Dictionary<TpFeature, bool> GetAllFeatures() =>
            Enum
                .GetValues(typeof(TpFeature))
                .Cast<TpFeature>()
                .Where(x => x != TpFeature.None)
                .ToDictionary(x => x, x => x.IsEnabled());

        public static bool AreEnabled(this ITpFeatureList list, params TpFeature[] features) => features.All(list.IsEnabled);

        public static bool IsEnabled(this TpFeature feature) => GetInstance().IsEnabled(feature);

        public static bool TryGetIsEnabled(this TpFeature feature, bool @default = false) =>
            TryGetInstance()?.IsEnabled(feature) ?? @default;

        public static bool IsSupported(this TpFeature _) => TryGetInstance() != null;

        public static bool IsDisabled(this TpFeature feature) => !IsEnabled(feature);

        public static bool AreAllEnabled(this ITpFeatureList featureList, params TpFeature[] features) =>
            features.All(featureList.IsEnabled);

        private static readonly Memoizator<TpFeature, string> _clientNameMemo = new Memoizator<TpFeature, string>(feature =>
        {
            return feature
                .GetAttribute<ClientFeatureAttribute>()
                .Select(x => x.ClientFeatureName)
                .GetOrElse(() => feature.AsString().CamelCase());
        });

        public static string GetClientName(this TpFeature feature)
        {
            return _clientNameMemo.Apply(feature);
        }

        private static readonly Memoizator<TpFeature, IReadOnlyList<string>> _mashupNamesMemo =
            new Memoizator<TpFeature, IReadOnlyList<string>>(feature =>
            {
                return feature.GetAttributes<MashupAttribute>().Select(x => x.MashupName).ToArray();
            });

        public static IEnumerable<string> GetMashupNames(this TpFeature feature)
        {
            return _mashupNamesMemo.Apply(feature);
        }

        private static ITpFeatureList GetInstance() => TpFeatureListFactory.GetInstance();

        private static ITpFeatureList TryGetInstance()
        {
            try
            {
                return GetInstance();
            }
            catch (StructureMapException)
            {
                return null;
            }
        }

        [PerformanceCritical]
        private static ITpFeatureListFactory TpFeatureListFactory
        {
            get
            {
                if (_tpFeatureListFactoryInitialized)
                {
                    return _tpFeatureListFactory;
                }

                _tpFeatureListFactory = ObjectFactory.GetInstance<ITpFeatureListFactory>();
                _tpFeatureListFactoryInitialized = true;
                return _tpFeatureListFactory;
            }
        }
    }
}
