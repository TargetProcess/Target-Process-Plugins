using System;
using Tp.Core.Diagnostics.Event;

namespace Tp.Core.Diagnostics.Time
{
    public class ProfilerDiagnostics
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
