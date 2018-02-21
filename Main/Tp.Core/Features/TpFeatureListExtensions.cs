using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace Tp.Core.Features
{
    public static class TpFeatureListExtensions
    {
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

        public static bool IsEnabled(this TpFeature feature) => ObjectFactory.GetInstance<ITpFeatureList>().IsEnabled(feature);
        public static bool IsDisabled(this TpFeature feature) => !IsEnabled(feature);

        public static bool IsAnyEnabled(this ITpFeatureList featureList, params TpFeature[] features) =>
            features.Any(featureList.IsEnabled);

        public static bool AreAllEnabled(this ITpFeatureList featureList, params TpFeature[] features) =>
            features.All(featureList.IsEnabled);

        public static string GetClientName(this TpFeature feature) =>
            feature
                .GetAttribute<ClientFeatureAttribute>()
                .Select(x => x.ClientFeatureName)
                .GetOrElse(() => feature.ToString().CamelCase());

        public static IEnumerable<string> GetMashupNames(this TpFeature feature) =>
            feature.GetAttributes<MashupAttribute>().Select(x => x.MashupName);

        public static bool CanUserCreateCustomMetrics(this ITpFeatureList features) =>
            features.IsEnabled(TpFeature.Metrics)
            && (features.IsEnabled(TpFeature.MetricsEffortViaRelations) || features.IsEnabled(TpFeature.MetricsCustomFormula));
    }
}
