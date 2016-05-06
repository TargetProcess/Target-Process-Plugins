using System;
using log4net;

namespace Tp.Core.Diagnostics.Event
{
	class DiagnosticEventsService : IDiagnosticEventsService
	{
		public Action<DiagnosticEvent> CreateEventWriter(string eventWriterName)
		{
			var logger = LogManager.GetLogger(eventWriterName);
			return e =>
			{
				try
				{
					var s = new DiagnosticEventSerializer();
					var serialized = s.Serialize(e);
					logger.Info(serialized);
				}
				catch (Exception error)
				{
					TpLogManager.Instance.DefaultLog.Error($"{eventWriterName} write failed.", error);
				}
			};
		}
	}
}