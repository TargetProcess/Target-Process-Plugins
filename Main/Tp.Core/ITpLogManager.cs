using log4net;

namespace Tp.Core
{
	public interface ITpLogManager
	{
		ILog GetLog(string loggerName);
		ILog DefaultLog { get; }
	}
}
