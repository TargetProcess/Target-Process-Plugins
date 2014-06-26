// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.Schemas;
using Tp.Bugzilla.Tests.Mocks;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization
{
	[TestFixture, ActionSteps]
	[Category("PartPlugins0")]
	public class CommentsFromTargetProcessSyncSpecs : BugzillaTestBase
	{
		[Test]
		public void ShouldImportComments()
		{
			@"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment 1' created on '2010-10-10 13:13' by 'Dowson@mail.com'

					And synchronizing bugzilla bugs
				When comment 'comment 2' for bug 'bug1' was created by 'Lansie' in TargetProcess
				Then bug 1 in bugzilla should have 2 comments
					And bug 1 in bugzilla should have comment 'comment 1' created by 'Dowson@mail.com'
					And bug 1 in bugzilla should have comment 'comment 2' created by 'Lansie@mail.com'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<CommentsFromTargetProcessSyncSpecs>()
								.And<CommentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldImportCommentsForAFewBugs()
		{
			@"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment 1' created on '2010-10-10 13:13' by 'Dowson@mail.com'
					
					And bugzilla contains bug with id 2
					And bug 2 has name 'bug2'

					And synchronizing bugzilla bugs
				When comment 'comment 2' for bug 'bug2' was created by 'Lansie' in TargetProcess
					And comment 'comment 2.1' for bug 'bug2' was created by 'Dowson' in TargetProcess
					And comment 'comment 1.1' for bug 'bug1' was created by 'Dowson' in TargetProcess
				Then bug 1 in bugzilla should have 2 comments
					And bug 1 in bugzilla should have comment 'comment 1' created by 'Dowson@mail.com'
					And bug 1 in bugzilla should have comment 'comment 1.1' created by 'Dowson@mail.com'
					And bug 2 in bugzilla should have 2 comments
					And bug 2 in bugzilla should have comment 'comment 2' created by 'Lansie@mail.com'
					And bug 2 in bugzilla should have comment 'comment 2.1' created by 'Dowson@mail.com'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<CommentsFromTargetProcessSyncSpecs>()
								.And<CommentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldNotImportCommentsForUnsyncBugs()
		{
			@"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment 1' created on '2010-10-10 13:13' by 'Dowson@mail.com'

					And synchronizing bugzilla bugs
				When comment 'comment 2' for unsync bug was created by 'Lansie' in TargetProcess
				Then bug 1 in bugzilla should have 1 comments
					And bug 1 in bugzilla should have comment 'comment 1' created by 'Dowson@mail.com'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<CommentsFromTargetProcessSyncSpecs>()
								.And<CommentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldNotImportDefaultComment()
		{
			string.Format(@"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment 1' created on '2010-10-10 13:13' by 'Lansie@mail.com'
					And synchronizing bugzilla bugs

				When comment '{0}' for bug 'bug1' was created by 'Lansie' in TargetProcess

				Then bug 1 in bugzilla should have 1 comments
			", CommentConverter.StateIsChangedComment)
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<CommentsFromTargetProcessSyncSpecs>()
								.And<CommentsFromBugzillaSyncSpecs>());
		}

		[Test]
		public void ShouldImportCommentsWithValidTimeOffset()
		{
			@"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla has timezone +5 UTC
					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'

					And synchronizing bugzilla bugs
				When comment 'comment 1' for bug 'bug1' was created on '2010-10-10 13:13:00+0300' by 'Lansie' in TargetProcess
				Then bug 1 in bugzilla should have 1 comments
					And bug 1 in bugzilla should have comment 'comment 1' created on '2010-10-10 15:13:00' by 'Lansie@mail.com'
			"
				.Execute(In.Context<BugSyncActionSteps>().And<BugSyncSpecs>().And<CommentsFromTargetProcessSyncSpecs>()
								.And<CommentsFromBugzillaSyncSpecs>());
		}

		[Given("bugzilla has timezone $timeOffset UTC")]
		public void SetBugzillaOffset(int timeOffset)
		{
			ObjectFactory.GetInstance<BugzillaServiceMock>().SetTimeOffset(new TimeSpan(timeOffset, 0, 0));
		}

		[When("comment '$commentText' for bug '$bugName' was created by '$owner' in TargetProcess")]
		public void CreateCommentInTargetProcess(string commentText, string bugName, string owner)
		{
			CreateCommentInTargetProcess(commentText, owner, Context.TpBugs.Single(b => b.Name == bugName).ID);
		}

		[When("comment '$commentText' for unsync bug was created by '$owner' in TargetProcess")]
		public void CreateCommentForUnsyncBug(string commentText, string owner)
		{
			CreateCommentInTargetProcess(commentText, owner, Context.GetNextId());
		}

		[When("comment '$commentText' for bug '$bugName' was created on '$createDate' by '$owner' in TargetProcess")]
		public void CreateCommentInTargetProcess(string commentText, string bugName, string createDate, string owner)
		{
			CreateCommentInTargetProcess(commentText, owner, Context.TpBugs.Single(b => b.Name == bugName).ID, DateTime.Parse(createDate));
		}

		[Then("bug $bugId in bugzilla should have $count comments")]
		public void CheckCommentsCount(int bugId, int count)
		{
			Context.BugzillaBugs.GetById(bugId).long_descCollection.Count.Should(Be.EqualTo(count + 1));
		}

		[Then("bug $bugId in bugzilla should have comment '$commentText' created by '$ownerEmail'")]
		public long_desc CheckBugComment(int bugId, string commentText, string ownerEmail)
		{
			Context.BugzillaBugs.GetById(bugId).long_descCollection.Cast<long_desc>().Where(c => c.thetext == commentText).ToList().Should
				(Be.Not.Empty);

			var comment =
				Context.BugzillaBugs.GetById(bugId).long_descCollection.Cast<long_desc>().Single(c => c.thetext == commentText);
			comment.who.Should(Be.EqualTo(ownerEmail));

			return comment;
		}

		[Then("bug $bugId in bugzilla should have comment '$commentText' without owner")]
		public void CheckCommentWithoutOwner(int bugId, string commentText)
		{
			CheckBugComment(bugId, commentText, null);
		}

		[Then("bug $bugId in bugzilla should have comment '$commentText' created on '$createDate' by '$ownerEmail'")]
		public void CheckBugComment(int bugId, string commentText, string createDate, string ownerEmail)
		{
			var comment = CheckBugComment(bugId, commentText, ownerEmail);

			comment.bug_when.Should(Be.EqualTo(DateTime.Parse(createDate).ToString("u")));
		}

		private void CreateCommentInTargetProcess(string commentText, string owner, int? bugId, DateTime createDate)
		{
			var comment = new CommentDTO
			              	{
			              		Description = commentText,
			              		OwnerID = Context.Users.Single(u => u.Login == owner).ID,
			              		GeneralID = bugId,
								CreateDate = createDate
			              	};

			TransportMock.HandleMessageFromTp(new CommentCreatedMessage
			                                  	{
			                                  		Dto = comment
			                                  	});
		}

		private void CreateCommentInTargetProcess(string commentText, string owner, int? bugId)
		{
			CreateCommentInTargetProcess(commentText, owner, bugId, DateTime.Now);
		}
	}
}
