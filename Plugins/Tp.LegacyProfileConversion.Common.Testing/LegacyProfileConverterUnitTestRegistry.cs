// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Reflection;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Integration.Plugin.Common.StructureMap;
using Tp.Integration.Plugin.Common.Tests;
using Tp.Integration.Testing.Common;
using Tp.LegacyProfileConvertsion.Common;

namespace Tp.LegacyProfileConversion.Common.Testing
{
	public abstract class LegacyProfileConverterUnitTestRegistry : Registry
	{
		protected LegacyProfileConverterUnitTestRegistry()
		{
			IncludeRegistry<PluginRegistry>();
			IncludeRegistry<NServiceBusMockRegistry>();
			For<IConvertorArgs>().HybridHttpOrThreadLocalScoped().Use(MockRepository.GenerateStub<IConvertorArgs>());
			For<IPluginContext>().HybridHttpOrThreadLocalScoped().Use(MockRepository.GenerateStub<IPluginContext>());
			For<IAssembliesHost>().Singleton().Use(new PredefinedAssembliesHost(new[] {PluginAssembly}));
			For<IProfileStoragePersister>().HybridHttpOrThreadLocalScoped().Use<ProfileStorageSqlPersister>();
		}

		protected abstract Assembly PluginAssembly { get; }
	}
}