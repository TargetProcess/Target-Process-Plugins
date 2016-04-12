// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Tp.Tfs.WorkItemsIntegration
{
	public interface IWorkItemsStore
	{
		WorkItem[] GetWorkItemsFrom(string workItemNumber);
		WorkItem[] GetWorkItemsFrom(DateTime from);
		WorkItem[] GetWorkItemsBetween(string projectName, string[] importedTypes, int minId, int maxId, DateTime lastSync);
		WorkItem GetWorkItem(int id);
	}
}
