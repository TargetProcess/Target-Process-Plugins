using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;

namespace Tp.Integration.Plugin.Common.Activity
{
	class PluginActivityLoggerFactory : IActivityLoggerFactory
	{
		private readonly ILogProvider _logProvider;
		public PluginActivityLoggerFactory(ILogProvider logProvider)
		{
			_logProvider = logProvider;
		}

		public IActivityLogger Create(IPluginContextSnapshot contextSnapshot)
		{
			return new PluginActivityLogger(_logProvider, Maybe.Return<IPluginContext>(contextSnapshot));
		}
	}
}