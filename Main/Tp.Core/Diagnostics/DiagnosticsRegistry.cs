using StructureMap.Configuration.DSL;
using Tp.Core.Diagnostics.Time;
using Tp.Core.Diagnostics.Time.Source;
using Tp.Core.Features;
using Tp.Core.Services;

namespace Tp.Core.Diagnostics
{
	public class DiagnosticsRegistry : Registry
	{
		public DiagnosticsRegistry()
		{
			For<IService>().Singleton().IfFeatureEnabled(TpFeature.Diagnostics).Use<PerformanceCounterService>().ElseUse<EmptyService>();
			For<Profiler>().HybridHttpOrThreadLocalScoped().Use<Profiler>();
			For<ITimePointsSource>().Singleton().Use<TimePointsSource>();
		}
	}
}
