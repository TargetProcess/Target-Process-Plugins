using System;
using log4net;

namespace Tp.Core.Diagnostics.Event
{
	class DiagnosticEventsService : IDiagnosticEventsService
	{
		private readonly ILog _log;
		public DiagnosticEventsService(ITpLogManager logManager)
		{
			_log = logManager.DefaultLog;
		}
		public Action<DiagnosticEvent> CreateEventReceiver(string eventWriterName)
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
					_log.Error($"{eventWriterName} write failed.", error);
				}
			};
		}
	}
}