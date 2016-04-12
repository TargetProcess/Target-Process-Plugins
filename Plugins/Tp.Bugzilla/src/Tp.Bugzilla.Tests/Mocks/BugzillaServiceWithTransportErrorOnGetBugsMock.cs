// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Bugzilla.Schemas;

namespace Tp.Bugzilla.Tests.Mocks
{
	public class BugzillaServiceWithTransportErrorOnGetBugsMock : BugzillaServiceMock, IBugzillaServiceFailMock
	{
		public bool Fail { set; get; }

		public override bugCollection GetBugs(int[] bugIDs)
		{
			if (Fail)
			{
				Fail = false;
				throw new Exception("Simulate error on getting bug chunk from Bugzilla.");
			}

			return base.GetBugs(bugIDs);
		}
	}
}