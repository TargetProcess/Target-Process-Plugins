// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.MashupManager.CustomCommands.Args;
using Tp.MashupManager.CustomCommands.Dtos;
using Tp.MashupManager.MashupLibrary.Package;
using Tp.MashupManager.MashupLibrary.Repository;
using Tp.MashupManager.MashupLibrary.Repository.Config;
using Tp.MashupManager.MashupLibrary.Repository.Exceptions;

namespace Tp.MashupManager.MashupLibrary
{
    public class Library : ILibrary
    {
        private readonly ILibraryRepositoryFactory _libraryRepositoryFactory;
        private readonly ILibraryRepositoryConfigStorage _configStorage;

        public Library(ILibraryRepositoryFactory libraryRepositoryFactory, ILibraryRepositoryConfigStorage configStorage)
        {
            _libraryRepositoryFactory = libraryRepositoryFactory;
            _configStorage = configStorage;
        }

        public virtual void Refresh()
        {
            _configStorage.GetConfigs().ForEach(config => _libraryRepositoryFactory.GetRepository(config).Refresh());
        }

        public virtual IEnumerable<LibraryRepositoryDto> GetRepositories()
        {
            return _configStorage.GetConfigs().Select(config => new LibraryRepositoryDto
            {
                Name = config.Name,
                Packages = GetPackages(config)
            });
        }

        private IEnumerable<LibraryPackage> GetPackages(ILibraryRepositoryConfig config)
        {
            return _libraryRepositoryFactory
                .GetRepository(config)
                .GetPackages();
        }

        public PackageDetailedDto GetPackageDetailed(PackageCommandArg commandArg)
        {
            var repositoryConfig = _configStorage.GetConfigs().FirstOrDefault(c => c.Name == commandArg.RepositoryName);
            if (repositoryConfig == null)
            {
                throw new RepositoryNotFoundException(commandArg.RepositoryName);
            }
            var repository = _libraryRepositoryFactory.GetRepository(repositoryConfig);
            var packageDetailed = repository.GetPackageDetailed(commandArg.PackageName);
            return new PackageDetailedDto
            {
                Name = packageDetailed.Name,
                ReadmeMarkdown = packageDetailed.ReadmeMarkdown,
                CompatibleTpVersionMinimum = packageDetailed.BaseInfo.CompatibleTpVersion.Minimum
            };
        }
    }
}
