// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus.Saga;
using Tp.Core;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Integration.Plugin.Common.StructureMap;
using Tp.Integration.Testing.Common.Persisters;

namespace Tp.Integration.Testing.Common
{
	public class AssemblyScannerMockRegistry : PluginRegistry
	{
		public AssemblyScannerMockRegistry()
			: this(typeof (AssemblyScannerMockRegistry).Assembly) {}

		public AssemblyScannerMockRegistry(Assembly pluginAssembly)
		{
			_pluginAssembly = pluginAssembly;
			Forward<ISagaPersister, TpInMemorySagaPersister>();

			Forward<IActivityLogger, LogMock>();
		}

		protected override IActivityLogger CreateActivityLogger()
		{
			return new LogMock();
		}

		private readonly Assembly _pluginAssembly;

		protected override IAssembliesHost GetAssembliesHost()
		{
			return new PredefinedAssembliesHost(new[] {_pluginAssembly});
		}

		protected override IProfileStoragePersister GetProfileStoragePersisterInstance()
		{
			return new ProfileStorageInMemoryPersister();
		}

		protected override IPluginPersister GetPluginPersisterInstance()
		{
			return new PluginInMemoryPersister();
		}

		protected override IAccountPersister GetAccountPersisterInstance()
		{
			return new AccountInMemoryPersister();
		}

		protected override IProfilePersister GetProfilePersisterInstance()
		{
			return new ProfileInMemoryPersister();
		}

		protected override ITaskFactory GetTaskFactory()
		{
			return new MockTaskFactory();
		}
	}

	public class MockTaskFactory : ITaskFactory
	{
		public Task StartNew(Action action)
		{
			action();
			return new Task(() => { });
		}
	}
}