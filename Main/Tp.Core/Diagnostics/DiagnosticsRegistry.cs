using StructureMap.Configuration.DSL;
using Tp.Core.Diagnostics.Counters;
using Tp.Core.Diagnostics.Event;
using Tp.Core.Diagnostics.Time;
using Tp.Core.Diagnostics.Time.Source;

namespace Tp.Core.Diagnostics
{
    public class DiagnosticsRegistry : Registry
    {
        public DiagnosticsRegistry()
        {
            For<Profiler>().HybridHttpOrThreadLocalScoped().Use(c => new Profiler("Empty"));
            For<ITimePointsSource>().Singleton().Use<TimePointsSource>();
            For<DiagnosticEventSerializer>().Use<DiagnosticEventSerializer>();
            For<IDiagnosticEventsService>().Singleton().Use<DiagnosticEventsService>();
            For<IDiagnosticCountersService>().Singleton().Use<DiagnosticCountersService>();
            For<ProfilerDiagnostics>().Singleton().Use<ProfilerDiagnostics>();
        }
    }
}
