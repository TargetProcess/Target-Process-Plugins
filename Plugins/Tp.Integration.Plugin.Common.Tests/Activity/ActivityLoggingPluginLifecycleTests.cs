// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using StructureMap;
using Tp.Testing.Common.NBehave;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class ActivityLoggingPluginLifecycleTests : ActivityTestBase
    {
        protected override void OnInit()
        {
            ObjectFactory.GetInstance<ActivityLoggingContext>().InitializeActivityLoggingMock();
        }

        [Test]
        public void ShouldRemoveLogsWhenDeletingProfile()
        {
            @"
				Given profile 'Profile' for account 'Account' created
				When plugin receives 'DeleteProfile' command for account 'Account' and profile 'Profile'
				Then logs for account 'Account' and profile 'Profile' should be removed
			"
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }

        [Test]
        public void ShouldNotLogBeforeProfileCreation()
        {
            @"
				Given profile 'Profile1' for account 'Account' created
				When plugin receives 'AddOrUpdateProfile' command for account 'Account' and profile 'Profile2'
				Then no records should be written to activity log for account 'Account' and profile 'Profile1'
			"
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }

        [Test]
        public void ShouldWriteToActivityLogDuringProfileUpdate()
        {
            @"
				Given profile 'Profile11' for account 'Account' created
					And profile 'Profile12' for account 'Account' created
				When plugin receives 'AddOrUpdateProfile' command for account 'Account' and profile 'Profile11'
				Then activity log for profile 'Profile11' for account 'Account' should be written
					And no records should be written to activity log for account 'Account' and profile 'Profile12'
			"
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }

        [Test]
        public void ShouldClearErrorLogWhenUpdatingProfile()
        {
            @"
				Given profile 'Profile' for account 'Account' created
				When plugin receives 'AddOrUpdateProfile' command for account 'Account' and profile 'Profile'
				Then error log for account 'Account' and profile 'Profile' should be removed
			"
                .Execute(In.Context<ActivityLoggingActionSteps>());
        }
    }
}
