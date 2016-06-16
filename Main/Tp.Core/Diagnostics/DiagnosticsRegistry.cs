using StructureMap.Configuration.DSL;
using Tp.Core.Diagnostics.Event;
using Tp.Core.Diagnostics.Time;
using Tp.Core.Diagnostics.Time.Source;
using Tp.Core.Services;

namespace Tp.Core.Diagnostics
{
	public class DiagnosticsRegistry : Registry
	{
		public DiagnosticsRegistry()
		{
			For<IService>().Singleton().Use<PerformanceCounterService>();
			For<Profiler>().HybridHttpOrThreadLocalScoped().Use<Profiler>();
			For<ITimePointsSource>().Singleton().Use<TimePointsSource>();
			For<DiagnosticEventSerializer>().Use<DiagnosticEventSerializer>();
			For<IDiagnosticEventsService>().Singleton().Use<DiagnosticEventsService>();
		}
	}
}
