// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NServiceBus;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.SourceControl.Workflow.Workflow;
using Tp.Tfs.Handlers;

namespace Tp.Tfs.StructureMap
{
	public class PluginSpecifyMessageHandlerOrdering : ICustomPluginSpecifyMessageHandlerOrdering
	{
		public void SpecifyHandlersOrder(First<PluginGateway> ordering)
		{
			ordering.AndThen<TfsWorkItemsListener>().AndThen<VersionControlSystemListener>();
		}
	}
}
