using Common.Logging;
using Common.Logging.Log4Net;

namespace Tp.Integration.Plugin.Common.Logging
{
	public class LoggerReferencer
	{
		public ILog Log { get; set; }
		public Log4NetLogger Logger { get; set; }
	}
}
