using System.Collections.Generic;
using Tp.MashupManager.MashupLibrary.Repository.Config;

namespace Tp.MashupManager.Tests.MashupLibrary.Repository.Config
{
	public class LibraryRepositoryConfigStorageMock : ILibraryRepositoryConfigStorage
	{
		public LibraryRepositoryConfigStorageMock()
		{
			Configs = new List<ILibraryRepositoryConfig>();
		}

		public void AddConfig(ILibraryRepositoryConfig config)
		{
			Configs.Add(config);
		}

		public IEnumerable<ILibraryRepositoryConfig> GetConfigs()
		{
			return Configs;
		}

		private List<ILibraryRepositoryConfig> Configs { get; set; }
	}
}