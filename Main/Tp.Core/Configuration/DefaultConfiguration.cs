using System.Configuration;

namespace Tp.Core.Configuration
{
	public class DefaultConfiguration : IConfigurationReader, IConfiguration
	{
		private DefaultConfiguration()
		{
		}

		public IConfiguration Config
		{
			get { return this; }
		}

		public static readonly IConfiguration Instance = new DefaultConfiguration();

		public T GetSection<T>(string sectionName) where T : ConfigurationSection
		{
			return (T) ConfigurationManager.GetSection(sectionName);
		}
	}
}
