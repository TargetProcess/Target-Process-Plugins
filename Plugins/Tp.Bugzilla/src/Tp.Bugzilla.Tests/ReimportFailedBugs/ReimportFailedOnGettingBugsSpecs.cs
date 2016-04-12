// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Bugzilla.Tests.Mocks;
using Tp.Bugzilla.Tests.Synchronization;
using Tp.Testing.Common.NBehave;

namespace Tp.Bugzilla.Tests.ReimportFailedBugs
{
	[TestFixture]
	[ActionSteps]
    [Category("PartPlugins0")]
	public class ReimportFailedOnGettingBugsSpecs :
		ReimportFailedBugsSpecsBase<BugzillaServiceWithTransportErrorOnGetBugsMock>
	{
		[Test]
		public void ShouldReimportBugsIfTransportErrorOccuredOnGettingBugs()
		{
			@"
				Given bugzilla profile created 
					And bugzilla contains bug 1 and name 'bug1' created on '2011-07-14 10:59:17'
					And bugzilla contains bug 2 and name 'bug2' created on '2011-07-14 10:59:17'
					And bugzilla contains bug 3 and name 'bug3' created on '2011-07-14 10:59:17'
					And chunk size is 1
					And last synchronization date is '2011-07-13 10:59:17'
				When transport error occured on getting bugs chunk during synchronization
					And last synchronization date is '2011-07-14 11:59:17'
					And synchronizing bugzilla bugs
				Then 3 bugs should be created in TargetProcess
					And bugs with following names should be created in TargetProcess: bug1, bug2, bug3
			"
				.Execute(
					In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<ReimportFailedOnGettingBugsSpecs>());
		}

		[Test]
		public void ShouldReimportBugsIfTransportErrorOccuredOnGettingBugsTwice()
		{
			@"
				Given bugzilla profile created 
					And bugzilla contains bug 1 and name 'bug1' created on '2011-07-14 10:59:17'
					And bugzilla contains bug 2 and name 'bug2' created on '2011-07-14 10:59:17'
					And bugzilla contains bug 3 and name 'bug3' created on '2011-07-14 10:59:17'
					And chunk size is 1
					And last synchronization date is '2011-07-13 10:59:17'
				When transport error occured on getting bugs chunk during synchronization
					And last synchronization date is '2011-07-14 11:59:17'
					And transport error occured on getting bugs chunk during synchronization
					And synchronizing bugzilla bugs
				Then 3 bugs should be created in TargetProcess
					And bugs with following names should be created in TargetProcess: bug1, bug2, bug3
			"
				.Execute(
					In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<ReimportFailedOnGettingBugsSpecs>());
		}

		[When("transport error occured on getting bugs chunk during synchronization")]
		public void ErrorWhenGetFirstBugsChunk()
		{
			FailSynchronization();
		}
	}
}