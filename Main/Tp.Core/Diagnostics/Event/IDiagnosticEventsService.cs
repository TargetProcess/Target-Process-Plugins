using System;

namespace Tp.Core.Diagnostics.Event
{
	public interface IDiagnosticEventsService
	{
		Action<DiagnosticEvent> CreateEventWriter(string eventWriterName);
	}
}