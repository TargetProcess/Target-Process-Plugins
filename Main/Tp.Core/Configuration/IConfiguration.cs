using System.Configuration;

namespace Tp.Core.Configuration
{
	public interface IConfiguration
	{
		T GetSection<T>(string sectionName) where T : ConfigurationSection;
	}
}
