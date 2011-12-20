// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Rhino.Mocks;
using StructureMap;
using StructureMap.Configuration.DSL;
using Tp.Integration.Messages;
using Tp.Integration.Messages.ServiceBus.Transport;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.PluginCommand;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Integration.Plugin.Common.Storage.Repositories;
using Tp.Integration.Plugin.Common.Tests.Common.ServiceBus;
using Tp.Integration.Testing.Common.Persisters;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	internal abstract class PluginStorageMockRegistry : Registry
	{
		protected PluginStorageMockRegistry()
		{
			For<IPluginSettings>().HybridHttpOrThreadLocalScoped().Use(MockRepository.GenerateStub<IPluginSettings>());

			For<IAccountRepository>().HybridHttpOrThreadLocalScoped().Use<AccountRepository>();

			For<ITpBus>().HybridHttpOrThreadLocalScoped().Use<TpBus>();

			IncludeRegistry<NServiceBusMockRegistry>();

			FillAllPropertiesOfType<ITpBus>();
			FillAllPropertiesOfType<IBus>();
			Forward<ITpBus, ILocalBus>();

			For<IPluginCurrentObjectContext>().HybridHttpOrThreadLocalScoped().Use<PluginCurrentObjectContext>();
			For<IAccountCollection>().Singleton().Use<AccountCollection>();
			For<IProfileCollectionReadonly>().Use<CurrentProfileCollection>();
			For<IProfileCollection>().Use<CurrentProfileCollection>();
			For<IProfile>().Use<CurrentProfile>();
			For<IProfileReadonly>().Use<CurrentProfile>();
			Forward<IProfileReadonly, IStorageRepository>();
			
			For<IAccountRepository>().Singleton().Use<AccountRepository>();
			For<IProfileRepository>().Singleton().Use<ProfileRepository>();
			Forward<IProfileRepository, IProfileFactory>();
			IncludeRegistry<PluginMetadataSourceMockRegistry>();

			For<IAccountRepository>().HybridHttpOrThreadLocalScoped().Use<AccountRepository>();
			For<IProfileRepository>().HybridHttpOrThreadLocalScoped().Use<ProfileRepository>();

			For<PluginRuntime>().Singleton().Use<PluginRuntime>();
			For<IEventAggregator>().Use(c => c.GetInstance<PluginRuntime>().EventAggregator);

			For<PluginContextMock>()
				.HybridHttpOrThreadLocalScoped()
				.Use(() => new PluginContextMock
				           	{
				           		PluginName =
				           			ObjectFactory.GetInstance<IPluginMetadata>().PluginData.Name,
				           		AccountName = AccountName.Empty,
				           		ProfileName = string.Empty
				           	});
			Forward<PluginContextMock, IPluginContext>();

			SetupPersisters();
			Forward<IPluginPersister, IProfileRepository>();

			For<IActivityLogPathProvider>().HybridHttpOrThreadLocalScoped().Use<ActivityLogPathProvider>();
			For<ILogManager>().HybridHttpOrThreadLocalScoped().Use<TpLogManager>();
			For<ILog4NetFileRepository>().HybridHttpOrThreadLocalScoped().Use<Log4NetFileRepository>();
			For<IActivityLogger>().HybridHttpOrThreadLocalScoped().Use<PluginActivityLogger>();

			For<IPluginQueueFactory>().HybridHttpOrThreadLocalScoped().Use<PluginQueueFactoryMock>();
		}

		protected abstract void SetupPersisters();
	}

	public class PluginMetadataSourceMockRegistry : Registry
	{
		public PluginMetadataSourceMockRegistry()
		{
			var metadataSource = MockRepository.GenerateStub<IPluginMetadata>();
			metadataSource.Stub(y => y.PluginData).Return(new PluginAssemblyAttribute("Plugin Name",
			                                                                                       "Plugin Description",
			                                                                                       "Plugin Category").GetData());
			metadataSource.Stub(y => y.ProfileType).Return(typeof (SampleJiraProfile));
			metadataSource.Stub(y => y.IsSyncronizableProfile).Return(true);
			For<IPluginMetadata>().HybridHttpOrThreadLocalScoped().Use(metadataSource);

			For<IPluginCommandRepository>().Singleton().Use<PluginCommandRepository>();
		}
	}

	internal class PluginStorageWithInMemoryPersisterMockRegistry : PluginStorageMockRegistry
	{
		protected override void SetupPersisters()
		{
			For<IPluginPersister>().HybridHttpOrThreadLocalScoped().Use(new PluginInMemoryPersister());
			For<IAccountPersister>().HybridHttpOrThreadLocalScoped().Use(new AccountInMemoryPersister());
			For<IProfilePersister>().HybridHttpOrThreadLocalScoped().Use(new ProfileInMemoryPersister());
			For<IProfileStoragePersister>().HybridHttpOrThreadLocalScoped().Use(new ProfileStorageInMemoryPersister());
		}
	}

	internal class PluginStorageWithSqlPersisterMockRegistry : PluginStorageWithInMemoryPersisterMockRegistry
	{
		protected override void SetupPersisters()
		{
			For<IPluginPersister>().HybridHttpOrThreadLocalScoped().Use<PluginPersister>();
			For<IProfilePersister>().HybridHttpOrThreadLocalScoped().Use<ProfilePersister>();
			For<IAccountPersister>().HybridHttpOrThreadLocalScoped().Use<AccountPersister>();

			For<IProfileStoragePersister>().HybridHttpOrThreadLocalScoped().Use<ProfileStorageSqlPersister>();
			For<IDatabaseConfiguration>().Use(MockRepository.GenerateStub<IDatabaseConfiguration>());
		}
	}
}