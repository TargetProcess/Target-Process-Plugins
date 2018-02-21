// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Bugzilla.Tests.Mocks
{
    public class BugzillaServiceWithTransportErrorOnGetIdsMock : BugzillaServiceMock, IBugzillaServiceFailMock
    {
        public bool Fail { get; set; }

        public override int[] GetChangedBugIds(DateTime? lastSyncDate)
        {
            if (Fail)
            {
                Fail = false;
                throw new Exception("Simulate error during getting changed ids from Bugzilla");
            }

            return base.GetChangedBugIds(lastSyncDate);
        }
    }
}
