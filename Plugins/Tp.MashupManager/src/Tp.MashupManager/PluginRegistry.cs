// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NGit.Util;
using NServiceBus;
using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.Integration.Plugin.Common.PluginCommand;
using Tp.MashupManager.MashupLibrary;
using Tp.MashupManager.MashupLibrary.Repository;
using Tp.MashupManager.MashupLibrary.Repository.Config;
using Tp.MashupManager.MashupLibrary.Repository.Synchronizer;
using Tp.MashupManager.MashupStorage;

namespace Tp.MashupManager
{
	public class PluginRegistry : Registry
	{
		public PluginRegistry()
		{
			For<ICustomPluginSpecifyMessageHandlerOrdering>().Singleton().Use
				<MashupManagerPluginSpecifyMessageHandlerOrdering>();
			For<IMashupInfoRepository>().Use<MashupInfoRepository>();
			For<ISingleProfile>().Singleton().Use<SingleProfile>();
			For<IMashupScriptStorage>().Use<MashupScriptStorage>();
			For<IMashupLocalFolder>().Use<MashupLocalFolder>();
			For<ILibrary>().Use<Library>();
			For<ILibraryLocalFolder>().Use<LibraryLocalFolder>();
			For<ILibraryRepositoryFactory>().Use<LibraryRepositoryFactory>();
			For<ILibraryRepositorySynchronizer>().Singleton().Use<LibraryRepositorySynchronizer>();
			For<ILibraryRepositoryConfigStorage>().Use<LibraryRepositoryConfigStorage>();
			For<IMashupLoader>().Use<MashupLoader>();
			
			SystemReader.SetInstance(new MockSystemReader(SystemReader.GetInstance()));
		}

		public class MashupManagerPluginSpecifyMessageHandlerOrdering : ICustomPluginSpecifyMessageHandlerOrdering
		{
			public void SpecifyHandlersOrder(First<PluginGateway> ordering)
			{
				ordering.AndThen<DeleteProfileCommandHandler>().AndThen<PluginCommandHandler>();
			}
		}
	}
}