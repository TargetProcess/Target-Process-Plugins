// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
	[TestFixture]
    [Category("PartPlugins1")]
    public class ShouldManipulatePluginProfilesActivityLogsTests : ActivityTestBase
	{
		[Test]
		public void ShouldCreateActivityLogForPluginProfile()
		{
			@"
				Given profile 'ProfileName1' for account 'AccountName1' created
				When retrieving activity logger
				Then activity logger for account 'AccountName1' and profile 'ProfileName1' should be retrieved
			"
				.Execute(In.Context<ActivityLoggingActionSteps>());
		}

		[Test]
		public void ShouldCreateErrorLogForPluginProfile()
		{
			@"
				Given profile 'ProfileName1' for account 'AccountName1' created
				When retrieving error logger
				Then error logger for account 'AccountName1' and profile 'ProfileName1' should be retrieved
			"
				.Execute(In.Context<ActivityLoggingActionSteps>());
		}

		[Test]
		public void ShouldCreateActivityLogForMultipleProfiles()
		{
			@"
				Given profile 'ProfileName21' for account 'AccountName21' created
					And profile 'ProfileName22' for account 'AccountName22' created
				When retrieving plugin activity logger for account 'AccountName21' and profile 'ProfileName21'
					and retrieving plugin activity logger for account 'AccountName22' and profile 'ProfileName22'
				Then activity logger for account 'AccountName21' and profile 'ProfileName21' should be retrieved
					and activity logger for account 'AccountName22' and profile 'ProfileName22' should be retrieved
			"
				.Execute(In.Context<ActivityLoggingActionSteps>());
		}

		[Test]
		public void ShouldCreateCommonLogWhenAccounNameIsNotSpecified()
		{
			@"
				Given profile 'ProfileName1' for empty account created
				When retrieving error logger
				Then error logger for account 'common' and profile 'ProfileName1' should be retrieved
			"
				.Execute(In.Context<ActivityLoggingActionSteps>());
		}

		[Test]
		public void ShouldRemoveLogsForProfile()
		{
			@"
				Given profile 'ProfileName1' for account 'AccountName1' created
				When removing activity logger for the profile
				Then logs for account 'AccountName1' and profile 'ProfileName1' should be removed
			"
				.Execute(In.Context<ActivityLoggingActionSteps>());
		}

		[Test]
		public void ShouldClearActivityLog()
		{
			@"
				Given profile 'ProfileName31' for account 'AccountName31' created
				When clearing activity log for the profile
				Then activity log for account 'AccountName31' and profile 'ProfileName31' should be removed
			"
				.Execute(In.Context<ActivityLoggingActionSteps>());
		}
	}
}