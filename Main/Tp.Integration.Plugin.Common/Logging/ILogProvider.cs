using System.Collections.Generic;
using Tp.Integration.Plugin.Common.Domain;
using log4net;

namespace Tp.Integration.Plugin.Common.Logging
{
	public interface ILogProvider
	{
		/// <summary>
		/// Gets all available loggers bound to runtime plugin context
		/// </summary>
		/// <returns></returns>
		IEnumerable<ILog> GetActivityLoggers(IPluginContext context);

		/// <summary>
		/// Gets all available loggers
		/// </summary>
		/// <returns></returns>
		IEnumerable<ILog> GetActivityLoggers();
	}
}