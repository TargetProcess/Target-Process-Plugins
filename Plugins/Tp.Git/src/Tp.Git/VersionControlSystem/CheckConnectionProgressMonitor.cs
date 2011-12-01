// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.

using NGit;

namespace Tp.Git.VersionControlSystem
{
	internal class CheckConnectionProgressMonitor : ProgressMonitor
	{
		public override void Start(int totalTasks)
		{
		}

		public override void BeginTask(string title, int totalWork)
		{
		}

		public override void Update(int completed)
		{
		}

		public override void EndTask()
		{
		}

		public override bool IsCancelled()
		{
			return true;
		}
	}
}