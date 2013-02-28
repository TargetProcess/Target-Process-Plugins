// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Globalization;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Tp.Tfs.WorkItemsIntegration
{
	public static class WorkItemExtensions
	{
		public static WorkItemId ToWorkItemId(this WorkItem workItem)
		{
			return new WorkItemId
			{
				Id = workItem.Id.ToString(CultureInfo.InvariantCulture),
			};
		}
	}
}
