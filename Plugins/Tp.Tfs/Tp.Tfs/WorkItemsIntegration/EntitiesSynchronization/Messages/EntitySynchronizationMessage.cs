// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Tfs.WorkItemsIntegration.EntitiesSynchronization.Messages
{
	public class EntitySynchronizationMessage : IPluginLocalMessage
	{
		public WorkItemInfo WorkItem { get; set; }
	}
}
