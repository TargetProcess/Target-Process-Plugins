using System;
using log4net;

namespace Tp.Core.Diagnostics.Event
{
    class DiagnosticEventsService : IDiagnosticEventsService
    {
        private readonly DiagnosticConfiguration _configuration;
        private readonly ILog _log;

        public DiagnosticEventsService(ITpLogManager logManager, DiagnosticConfiguration configuration)
        {
            _configuration = configuration;
            _log = logManager.DefaultLog;
        }

        public Action<DiagnosticEvent> CreateEventReceiver(string eventWriterName)
        {
            var logger = LogManager.GetLogger(eventWriterName);
            return e =>
            {
                try
                {
                    if (e.IsDetail && !_configuration.ShouldTraceDetails)
                    {
                        return;
                    }

                    if (logger.IsInfoEnabled)
                    {
                        var s = new DiagnosticEventSerializer();
                        var serialized = s.Serialize(e);
                        logger.Info(serialized);
                    }
                }
                catch (Exception error)
                {
                    _log.Error($"{eventWriterName} write failed.", error);
                }
            };
        }
    }
}
