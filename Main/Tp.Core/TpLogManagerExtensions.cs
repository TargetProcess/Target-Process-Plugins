using System;
using log4net;

namespace Tp.Core
{
	public static class TpLogManagerExtensions
	{
		public static ILog GetLog(this ITpLogManager logManager, Type type)
		{
			return logManager.GetLog(type.FullName);
		}

		public static ILog PerformanceCounterLog(this ITpLogManager logManager)
		{
			return logManager.GetLog("PerformanceCounter");
		}
	}
}
