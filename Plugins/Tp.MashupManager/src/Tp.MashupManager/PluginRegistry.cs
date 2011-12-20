// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.Integration.Plugin.Common.PluginCommand;
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