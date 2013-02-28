// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Tfs.WorkItemsIntegration
{
	public class WorkItemsTypes
	{
		private static WorkItemsTypes _instance;
		private static readonly object LockObject = new object();

		private WorkItemsTypes()
		{
			TypesTfs2010 = new[] { "Task", "User Story", "Bug", "Issue" };
			TypesTfs2012 = new[] { "Task", "Product Backlog Item", "Bug", "Impediment" };
			AllTypes = new[] { "Task", "User Story", "Bug", "Issue", "Product Backlog Item", "Impediment" };
		}

		public static WorkItemsTypes Instance
		{
			get
			{
				lock (LockObject)
				{
					if (_instance == null)
						_instance = new WorkItemsTypes();
				}

				return _instance;
			}
		}

		public string[] AllTypes { get; set; }

		public string[] TypesTfs2010 { get; set; }

		public string[] TypesTfs2012 { get; set; }
	}
}
