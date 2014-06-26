// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.CustomCommands.Args;
using Tp.MashupManager.MashupLibrary.Repository;
using Tp.MashupManager.MashupLibrary.Repository.Config;

namespace Tp.MashupManager.CustomCommands
{
	public class InstallPackageCommand : LibraryCommand<PackageCommandArg>
	{
		private readonly IMashupInfoRepository _mashupInfoRepository;
		private readonly ILibraryRepositoryConfigStorage _libraryRepositoryConfigStorage;
		private readonly ILibraryRepositoryFactory _libraryRepositoryFactory;

		public InstallPackageCommand(IMashupInfoRepository mashupInfoRepository, ILibraryRepositoryConfigStorage libraryRepositoryConfigStorage, 
			ILibraryRepositoryFactory libraryRepositoryFactory)
		{
			_mashupInfoRepository = mashupInfoRepository;
			_libraryRepositoryConfigStorage = libraryRepositoryConfigStorage;
			_libraryRepositoryFactory = libraryRepositoryFactory;
		}

		protected override PluginCommandResponseMessage ExecuteOperation(PackageCommandArg commandArg)
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

			ILibraryRepository repository = _libraryRepositoryFactory.GetRepository(config);
			Mashup mashup = repository.GetPackageMashup(commandArg.PackageName);			
			PluginProfileErrorCollection errors = _mashupInfoRepository.Add(mashup, true);

			return errors.Empty()
				       ? new PluginCommandResponseMessage {PluginCommandStatus = PluginCommandStatus.Succeed}
				       : new PluginCommandResponseMessage
					       {
						       PluginCommandStatus = PluginCommandStatus.Error,
						       ResponseData = errors.Aggregate("", (acc, e) => acc + e.Message + "\r\n")
					       };
		}

		public override string Name
		{
			get { return "InstallPackage"; }
		}
	}
}