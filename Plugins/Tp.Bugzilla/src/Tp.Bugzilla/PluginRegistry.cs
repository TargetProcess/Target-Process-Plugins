// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using NServiceBus.Sagas.Impl;
using StructureMap.Configuration.DSL;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.ImportToTp;
using Tp.Bugzilla.Mappers;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.Plugin.Core.Attachments;

namespace Tp.Bugzilla
{
	public class PluginRegistry : Registry
	{
		public PluginRegistry()
		{
			For<ICustomPluginSpecifyMessageHandlerOrdering>().Singleton().Use<BugzillaPluginSpecifyMessageHandlerOrdering>();
			For<IBugzillaService>().Use<BugzillaService>();
			For<IBugChunkSize>().Singleton().Use<BugChunkSize>();
			For<IBufferSize>().Singleton().Use<BufferSize>();
			For<IBugConverter>().Singleton().Use<ConverterComposite>();
			For<IBugzillaInfoStorageRepository>().Use<BugzillaInfoStorageRepository>();
			For<IBugzillaActionFactory>().Use<BugzillaActionFactory>();
			For<IUserMapper>().Use<UserMapper>();
			For<IBugzillaFieldsMapper>().Use<BugzillaFieldsMapper>();
		}
	}

	public class BugzillaPluginSpecifyMessageHandlerOrdering : ICustomPluginSpecifyMessageHandlerOrdering
	{
		//Here we specify that sagas will always be executed before message hanlders.
		public void SpecifyHandlersOrder(First<PluginGateway> ordering)
		{
			ordering.AndThen<SagaMessageHandler>();
		}
	}
}