using StructureMap.Configuration.DSL;
using Tp.Core.Diagnostics.Time;
using Tp.Core.Features;
using Tp.Core.Services;

namespace Tp.Core.Diagnostics
{
	public class DiagnosticsRegistry : Registry
	{
		public DiagnosticsRegistry()
		{
			For<Diagnostics>().HybridHttpOrThreadLocalScoped().Use<Diagnostics>();
			For<IService>().Singleton().IfFeatureEnabled(TpFeature.Diagnostics).Use<PerformanceCounterService>().ElseUse<EmptyService>();
			For<TpProfiler>().HybridHttpOrThreadLocalScoped().Use<TpProfiler>();
		}
	}
}
