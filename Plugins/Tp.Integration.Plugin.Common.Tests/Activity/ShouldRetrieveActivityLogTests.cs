// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Core;
using Tp.Testing.Common.NBehave;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
    [Category("PartPlugins1")]
    public class ShouldRetrieveActivityLogTests : ActivityTestBase
    {
        [Test]
        public void ShouldRetrieveActivityLogs()
        {
            @"
				Given profile 'ProfileName1' for account 'AccountName1' created
				When retrieving activity logs
				Then activity object with 3 records should be retrieved
			"
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }

        [Test]
        public void ShouldRetrieveActivityLogsByEndDate()
        {
            string.Format(
                    @"
					Given profile 'ProfileName1' for account 'AccountName1' created
					When retrieving activity logs by end date '{0:yyyy-MM-dd hh:mm:ss,fff tt}'
					Then activity object with 3 records should be retrieved
				",
                    CurrentDate.Value.AddDays(1))
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }

        [Test]
        public void ShouldRetrieveActivityLogsByStartDate()
        {
            string.Format(
                    @"
					Given profile 'ProfileName1' for account 'AccountName1' created
					When retrieving activity logs by start date '{0:yyyy-MM-dd hh:mm:ss,fff tt}'
					Then activity object with 6 records should be retrieved
				",
                    CurrentDate.Value.AddMonths(-1).AddSeconds(-1))
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }

        [Test]
        public void ShouldRetrieveNoRecordsForTooOldEndDate()
        {
            string.Format(
                    @"
					Given profile 'ProfileName1' for account 'AccountName1' created
					When retrieving activity logs by end date '{0:yyyy-MM-dd hh:mm:ss,fff tt}'
					Then no records should be retrieved
				",
                    CurrentDate.Value.AddMonths(-3))
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }

        [Test]
        public void ShouldRetrieveErrorLogs()
        {
            @"
				Given profile 'ProfileName1' for account 'AccountName1' created
				When retrieving error logs
				Then activity object with 3 records should be retrieved
			"
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }
    }
}
