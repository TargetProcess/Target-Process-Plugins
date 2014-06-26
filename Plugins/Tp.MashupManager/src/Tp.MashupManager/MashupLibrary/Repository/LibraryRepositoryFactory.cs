// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.MashupManager.MashupLibrary.Repository.Config;
using Tp.MashupManager.MashupLibrary.Repository.Synchronizer;

namespace Tp.MashupManager.MashupLibrary.Repository
{
	public class LibraryRepositoryFactory : ILibraryRepositoryFactory
	{
		private readonly ILibraryLocalFolder _folder;
		private readonly ILibraryRepositorySynchronizer _synchronizer;
		private readonly IMashupLoader _mashupLoader;

		public LibraryRepositoryFactory(ILibraryLocalFolder folder, ILibraryRepositorySynchronizer synchronizer, IMashupLoader mashupLoader)
		{
			_folder = folder;
			_synchronizer = synchronizer;
			_mashupLoader = mashupLoader;
		}

		public ILibraryRepository GetRepository(ILibraryRepositoryConfig config)
		{
			return new LibraryRepository(config, _folder, _synchronizer, _mashupLoader);
		}
	}
}