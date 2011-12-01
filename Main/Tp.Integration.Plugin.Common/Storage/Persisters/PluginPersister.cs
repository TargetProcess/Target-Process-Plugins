using System.Linq;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
	public interface IPluginPersister
	{
		void CreateIfMissing();
	}

	internal class PluginPersister : IPluginPersister
	{
		private readonly IDatabaseConfiguration _configuration;
		private readonly string _pluginName;

		public PluginPersister(IDatabaseConfiguration configuration, IPluginMetadata pluginMetadata)
		{
			_configuration = configuration;
			_pluginName = pluginMetadata.PluginData.Name;
		}

		public void CreateIfMissing()
		{
			using(var context = new PluginDatabaseModelDataContext(_configuration.ConnectionString))
			{
				Plugin plugin = context.Plugins.SingleOrDefault(p => p.Name == _pluginName);
				if (plugin != null)
				{
					return;
				}
				plugin = new Plugin { Name = _pluginName };
				context.Plugins.InsertOnSubmit(plugin);
				context.SubmitChanges();
			}
		}
	}
}