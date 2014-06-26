// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Integration
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins0")]
	public class TargetProcessToBugzillaSyncronizationSpecs : BugzillaTestBase
	{
		private string _bugzillaResponse;

		[SetUp]
		public void Setup()
		{
			ObjectFactory.Configure(
				x =>
				x.For<TransportMock>().HybridHttpOrThreadLocalScoped().Use(
					TransportMock.CreateWithoutStructureMapClear(typeof (BugzillaProfile).Assembly)));
		}

		[Test, Ignore]
		public void ShouldChangeAssignment()
		{
			@"
				Given bugzilla profile created
				When change assignment for bug 27 to 'bugzilla@targetprocess.com' at bugzilla
				Then bugzilla should return successful message
			"
				.Execute();
		}

		[Test]
		public void ShouldNotChangeAssignmentWithInvalidBugId()
		{
			@"
				Given bugzilla profile created
				When change assignment for bug 1055555 to 'bugzilla@targetprocess.com' at bugzilla
				Then bugzilla should return fail message
			"
				.Execute();
		}

		[Test]
		public void ShouldNotChangeAssignmentWithInvalidUser()
		{
			@"
				Given bugzilla profile created
				When change assignment for bug 27 to 'jack@mail.com1' at bugzilla
				Then bugzilla should return Invalid User message
			"
				.Execute();
		}

		[Test, Ignore]
		public void ShouldCreateComment()
		{
			@"
				Given bugzilla profile created
				When create comment 'comment text' for bug bug 27 by 'bugzilla@targetprocess.com' for '2010-11-11'
				Then bugzilla should return successful message
			"
				.Execute();
		}

		[Test]
		public void ShouldNotCreateCommentForInvalidBug()
		{
			@"
				Given bugzilla profile created
				When create comment 'comment text' for bug bug 1055555 by 'bugzilla@targetprocess.com' for '2010-11-11'
				Then bugzilla should return fail message
			"
				.Execute();
		}

		[Test, Ignore]
		public void ShouldCreateCommentForInvalidOwner()
		{
			@"
				Given bugzilla profile created
				When create comment 'comment text' for bug bug 27 by 'jack@mail.com1' for '2010-11-11'
				Then bugzilla should return successful message
			"
				.Execute();
		}

		[Given("bugzilla profile created")]
		public void CreateProfile()
		{
			Context.CreateDefaultRolesIfNecessary();
			Context.AddProfileWithDefaultRolesMapping(1, new BugzillaProfile
			                                             	{
																Url = BugzillaTestConstants.Url,
																Login = BugzillaTestConstants.Login,
																Password = BugzillaTestConstants.Password,
																SavedSearches = BugzillaTestConstants.Queries,
			                                             		Project = 1
			                                             	});
		}

		[When("change assignment for bug $bugId to '$userEmail' at bugzilla")]
		public void ChangeAssignmentInBugzilla(int bugId, string userEmail)
		{
			_bugzillaResponse =
				new BugzillaUrl(Profile.GetProfile<BugzillaProfile>()).ExecuteOnBugzilla(new BugzillaAssigneeAction(
				                                                                         	bugId.ToString(), userEmail));
		}

		[When("create comment '$comment' for bug bug $bugId by '$ownerEmail' for '$createDate'")]
		public void CreateComment(string comment, int bugId, string ownerEmail, string createDate)
		{
			_bugzillaResponse =
				new BugzillaUrl(Profile.GetProfile<BugzillaProfile>()).ExecuteOnBugzilla(new BugzillaCommentAction(
				                                                                         	bugId.ToString(), comment, ownerEmail,
				                                                                         	DateTime.Parse(createDate)));
		}

		[Then("bugzilla should return successful message")]
		public void CheckBugzillaReturnSuccess()
		{
			_bugzillaResponse.Should(Be.EqualTo("OK"));
		}

		[Then("bugzilla should return fail message")]
		public void CheckBugzillaReturnedFail()
		{
			_bugzillaResponse.Should(Be.Not.EqualTo("OK"));
			_bugzillaResponse.Contains("Invalid User").Should(Be.False);
		}

		[Then("bugzilla should return Invalid User message")]
		public void CheckBugzillaReturnedInvalidUser()
		{
			_bugzillaResponse.Contains("Invalid User").Should(Be.True);
		}
	}
}