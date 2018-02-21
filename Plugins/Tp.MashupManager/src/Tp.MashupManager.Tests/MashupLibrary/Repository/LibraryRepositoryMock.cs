using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.MashupManager.CustomCommands.Dtos;
using Tp.MashupManager.MashupLibrary;
using Tp.MashupManager.MashupLibrary.Package;
using Tp.MashupManager.MashupLibrary.Repository;
using Tp.MashupManager.MashupLibrary.Repository.Config;

namespace Tp.MashupManager.Tests.MashupLibrary.Repository
{
    public class LibraryRepositoryMock : ILibraryRepository
    {
        public LibraryRepositoryMock(ILibraryRepositoryConfig config)
        {
            Config = config;
            Sources = new List<LibraryRepositoryDto>();
            RefreshedLibraries = new List<ILibraryRepositoryConfig>();
            PackagesDetailed = new List<LibraryPackageDetailed>();
        }

        public List<LibraryRepositoryDto> Sources { get; set; }
        public List<ILibraryRepositoryConfig> RefreshedLibraries { get; set; }
        public ILibraryRepositoryConfig Config { get; private set; }
        public List<LibraryPackageDetailed> PackagesDetailed { get; set; }

        public IEnumerable<LibraryPackage> GetPackages()
        {
            return Sources
                .Single(s => s.Name == Config.Name).Packages
                .Select(
                    x =>
                        new LibraryPackage
                        {
                            Name = x.Name,
                            BaseInfo = new LibraryPackageBaseInfo()
                        });
        }

        public Mashup GetPackageMashup(string packageName)
        {
            return null;
        }

        public LibraryPackageDetailed GetPackageDetailed(string packageName)
        {
            return PackagesDetailed.Single(x => x.Name == packageName);
        }

        public void Refresh()
        {
            RefreshedLibraries.Add(Config);
        }

        public void Init()
        {
            ObjectFactory.Configure(x =>
                {
                    x.For<ILibrary>().Singleton().Use<Library>();
                    x.For<ILibraryRepository>().Singleton().Use(this);
                }
            );
        }
    }
}
