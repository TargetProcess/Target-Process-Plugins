using System;
using System.Linq;
using Tp.Integration.Messages.Commands;
using Tp.MashupManager.CustomCommands.Args;
using Tp.MashupManager.MashupLibrary.Repository;
using Tp.MashupManager.MashupLibrary.Repository.Config;

namespace Tp.MashupManager.CustomCommands
{
    public class InstallPackageCommand : LibraryCommand<InstallPackageCommandArg>
    {
        private readonly IMashupInfoRepository _mashupInfoRepository;
        private readonly ILibraryRepositoryConfigStorage _libraryRepositoryConfigStorage;
        private readonly ILibraryRepositoryFactory _libraryRepositoryFactory;

        public InstallPackageCommand(IMashupInfoRepository mashupInfoRepository,
            ILibraryRepositoryConfigStorage libraryRepositoryConfigStorage,
            ILibraryRepositoryFactory libraryRepositoryFactory)
        {
            _mashupInfoRepository = mashupInfoRepository;
            _libraryRepositoryConfigStorage = libraryRepositoryConfigStorage;
            _libraryRepositoryFactory = libraryRepositoryFactory;
        }

        protected override PluginCommandResponseMessage ExecuteOperation(InstallPackageCommandArg commandArg)
        {
            var config = _libraryRepositoryConfigStorage.GetConfigs().FirstOrDefault(c => c.Name == commandArg.RepositoryName);
            if (config == null)
            {
                return new PluginCommandResponseMessage
                {
                    PluginCommandStatus = PluginCommandStatus.Error,
                    ResponseData = "Unknown repository {0}".Fmt(commandArg.RepositoryName)
                };
            }

            var repository = _libraryRepositoryFactory.GetRepository(config);
            var mashup = repository.GetPackageMashup(commandArg.PackageName);
            mashup.MashupMetaInfo.PackageName = commandArg.PackageName;
            mashup.MashupMetaInfo.CreationDate = commandArg.CreationDate;
            mashup.MashupMetaInfo.CreatedBy = commandArg.CreatedBy;
            var errors = _mashupInfoRepository.Add(mashup, true);

            return errors.Empty()
                ? new PluginCommandResponseMessage { PluginCommandStatus = PluginCommandStatus.Succeed }
                : new PluginCommandResponseMessage
                {
                    PluginCommandStatus = PluginCommandStatus.Error,
                    ResponseData = errors.Aggregate("", (acc, e) => acc + e.Message + "\r\n")
                };
        }

        public override string Name => "InstallPackage";
    }
}
