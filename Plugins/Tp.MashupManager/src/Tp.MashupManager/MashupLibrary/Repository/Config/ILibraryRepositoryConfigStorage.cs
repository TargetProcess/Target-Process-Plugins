using System.Collections.Generic;

namespace Tp.MashupManager.MashupLibrary.Repository.Config
{
    public interface ILibraryRepositoryConfigStorage
    {
        IEnumerable<ILibraryRepositoryConfig> GetConfigs();
    }
}
