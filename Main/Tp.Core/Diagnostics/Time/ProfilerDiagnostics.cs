using System;
using Tp.Core.Diagnostics.Event;

namespace Tp.Core.Diagnostics.Time
{
    public interface IProfilerDiagnostics
    {
        void Track(DiagnosticEvent diagnosticEvent);
    }

    public class ProfilerDiagnostics
        : IProfilerDiagnostics
    {
        private readonly Action<DiagnosticEvent> _receiver;

        public ProfilerDiagnostics(IDiagnosticEventsService eventsService)
        {
            _receiver = eventsService.CreateEventReceiver("ProfilerStatisticsDetailed");
        }

        public void Track(DiagnosticEvent diagnosticEvent)
        {
            _receiver(diagnosticEvent);
        }
    }
}
