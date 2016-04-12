using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace Tp.Core.Features
{
	public static class TpFeatureListExtensions
	{
		public static TpFeature[] GetEnabledFeatures(this ITpFeatureList list)
		{
			return Enum.GetValues(typeof(TpFeature)).Cast<TpFeature>().Where(list.IsEnabled).ToArray();
		}

		public static Dictionary<TpFeature, bool> GetAllFeatures()
		{
			return Enum.GetValues(typeof(TpFeature)).Cast<TpFeature>().Where(x => x != TpFeature.None).ToDictionary(x => x, x => x.IsEnabled());
		}

		public static bool IsEnabled(this TpFeature feature)
		{
			return ObjectFactory.GetInstance<ITpFeatureList>().IsEnabled(feature);
		}

		public static string GetClientName(this TpFeature feature)
		{
			return feature.GetAttribute<ClientFeatureAttribute>().Select(x => x.ClientFeatureName).GetOrElse(() => feature.ToString().CamelCase());
		}

		public static IEnumerable<string> GetMashupNames(this TpFeature feature)
		{
			return feature.GetAttributes<MashupAttribute>().Select(x => x.MashupName);
		}
	}
}
