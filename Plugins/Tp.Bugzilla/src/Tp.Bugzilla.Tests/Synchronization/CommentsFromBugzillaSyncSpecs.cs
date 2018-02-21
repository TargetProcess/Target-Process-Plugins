// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Bugzilla.Schemas;
using Tp.Bugzilla.Tests.Mocks;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization
{
    [TestFixture]
    [ActionSteps]
    [Category("PartPlugins1")]
    public class CommentsFromBugzillaSyncSpecs : BugzillaTestBase
    {
        private const string DefaultCreateDate = "2011-07-14 10:59:17";

        [Test]
        public void ShouldImportCommentsOnBugCreated()
        {
            @"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment 1' created on '2011-07-14 10:59:17' by 'Lansie@mail.com'
					And bug 1 has comment 'comment 2' created on '2011-07-14 10:59:17' by 'Dowson@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 2 comments
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;1' created on '2011-07-14 10:59:17' by 'Lansie'
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;2' created on '2011-07-14 10:59:17' by 'Dowson'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<CommentsFromBugzillaSyncSpecs>());
        }

        [Test]
        public void ShouldImportCommentsWithoutOwner()
        {
            @"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment 1' created on '2011-07-14 10:59:17' by 'Lansie@mail.com'
					And bug 1 has comment 'comment 2' created on '2011-07-14 10:59:17' by 'Someone@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 2 comments
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;1' created on '2011-07-14 10:59:17' by 'Lansie'
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;2' created on '2011-07-14 10:59:17' by default user
			"
                .Execute(In.Context<BugSyncActionSteps>().And<CommentsFromBugzillaSyncSpecs>());
        }

        [Test]
        public void ShouldNotImportCommentIfThereIsNoComments()
        {
            @"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 0 comments
			"
                .Execute(In.Context<BugSyncActionSteps>().And<CommentsFromBugzillaSyncSpecs>());
        }

        [Test]
        public void ShouldAddOnlyNewCommentOnBugUpdated()
        {
            @"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment 1' created on '2011-07-14 10:59:17' by 'Lansie@mail.com'
					And bug 1 has comment 'comment 2' created on '2011-07-14 10:59:18' by 'Dowson@mail.com'
					And synchronizing bugzilla bugs
					
					And bug 1 has comment 'comment 3' created on '2011-07-14 10:59:19' by 'Dowson@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 3 comments
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;1' created on '2011-07-14 10:59:17' by 'Lansie'
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;2' created on '2011-07-14 10:59:18' by 'Dowson'
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;3' created on '2011-07-14 10:59:19' by 'Dowson'
					And import should be completed
			"
                .Execute(In.Context<BugSyncActionSteps>().And<CommentsFromBugzillaSyncSpecs>());
        }

        [Test]
        public void ShouldClearCommentOfInvalidCharacters()
        {
            @"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment with invalid char at the end 'comment 1' created by 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 1 comments
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;1' created by 'Lansie'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<CommentsFromBugzillaSyncSpecs>());
        }

        [Test]
        public void ShouldNotClearAllowedCharacters()
        {
            @"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment	 1 
' created on '2011-07-14 10:59:17' by 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 1 comments
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;1&nbsp;<br/>' created by 'Lansie'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<CommentsFromBugzillaSyncSpecs>());
        }

        [Test]
        public void ShouldAssignCommentWhenFewBugzillaUsersMappedToSingleTargetProcessUser()
        {
            @"
				Given user 'Lansie' with email 'Lansie@mail.com' created in TargetProcess
					And user 'Dowson' with email 'Dowson@mail.com' created in TargetProcess

					And bugzilla profile for project 1 created
					And user mapping added:
					|bugzillaEmail|targetProcessLogin|
					|BugzillaUser@mail.com|Dowson|
					|Lansie@mail.com|Dowson|

					And bugzilla contains bug with id 1
					And bug 1 has name 'bug1'
					And bug 1 has comment 'comment 1' created on '2011-07-14 10:59:17' by 'Lansie@mail.com'
				When synchronizing bugzilla bugs
				Then bugs with following names should be created in TargetProcess: bug1
					And bug in TargetProcess with name 'bug1' should have 1 comments
					And bug in TargetProcess with name 'bug1' should have comment 'comment&nbsp;1' created by 'Dowson'
			"
                .Execute(In.Context<BugSyncActionSteps>().And<CommentsFromBugzillaSyncSpecs>());
        }

        [Given("bug $bugId has comment '$commentText' created on '$createDate' by '$ownerEmail' ")]
        public void AddComment(int bugId, string commentText, string createDate, string ownerEmail)
        {
            var comment = new long_desc { thetext = commentText, bug_when = createDate, who = ownerEmail };
            ObjectFactory.GetInstance<BugzillaServiceMock>().Bugs.AddComment(bugId, comment);
        }

        [Given("bug $bugId has comment '$commentText' created by '$ownerEmail'")]
        public void AddComment(int bugId, string commentText, string ownerEmail)
        {
            AddComment(bugId, commentText, DefaultCreateDate, ownerEmail);
        }

        [Given("bug $bugId has comment with invalid char at the end '$commentText' created by '$ownerEmail'")]
        public void AddCommentWithInvalidChar(int bugId, string commentText, string ownerEmail)
        {
            AddComment(bugId, commentText + ((char) 8).ToString(), ownerEmail);
        }

        [Then("bug in TargetProcess with name '$bugName' should have $commentsCount comments")]
        public void CheckCommentsCount(string bugName, int commentsCount)
        {
            TransportMock.TpQueue.GetMessages<CreateCommand>().Where(c => c.Dto is CommentDTO).Count()
                .Should(Be.EqualTo(commentsCount),
                    "TransportMock.TpQueue.GetMessages<CreateCommand>().Where(c => c.Dto is CommentDTO).Count().Should(Be.EqualTo(commentsCount))");
        }

        [Then(
             "bug in TargetProcess with name '$bugName' should have comment '$commentText' created on '$createDate' by '$tpLogin'"
         )]
        public void CheckBugComment(string bugName, string commentText, string createdDate, string tpLogin)
        {
            CheckBugCommentExists(bugName, commentText, createdDate, Context.Users.Single(u => u.Login == tpLogin).ID);
        }

        [Then(
             "bug in TargetProcess with name '$bugName' should have comment '$commentText' created on '$createDate' by default user"
         )]
        public void CheckBugCommentForDefaultUser(string bugName, string commmentText, string createdDate)
        {
            CheckBugCommentExists(bugName, commmentText, createdDate, null);
        }

        [Then("bug in TargetProcess with name '$bugName' should have comment '$commentText' created by '$tpLogin'")]
        public void CheckBugComment(string bugName, string commentText, string tpLogin)
        {
            CheckBugComment(bugName, commentText, DefaultCreateDate, tpLogin);
        }

        private void CheckBugCommentExists(string bugName, string commmentText, string createdDate, int? ownerId)
        {
            Context.TpComments
                .Where(c => c.Description == commmentText)
                .Where(c => c.CreateDate == DateTime.Parse(createdDate).ToLocalTime())
                .Where(c => c.OwnerID == ownerId)
                .Where(c => c.GeneralID == Context.TpBugs.Single(b => b.Name == bugName).ID)
                .Any()
                .Should(Be.True,
                    "Context.TpComments.Where(c => c.Description == commmentText).Where(c => c.CreateDate == DateTime.Parse(createdDate).ToLocalTime()).Where(c => c.OwnerID == ownerId).Where(c => c.GeneralID == Context.TpBugs.Single(b => b.Name == bugName).ID).Any().Should(Be.True)");
        }
    }
}
