// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Integration
{
	[TestFixture, ActionSteps]
	public class CheckConnectionTest : BugzillaTestBase
	{
		private readonly PluginProfileErrorCollection _errors = new PluginProfileErrorCollection();

		[SetUp]
		public void Setup()
		{
			ObjectFactory.Configure(
				x =>
				x.For<TransportMock>().HybridHttpOrThreadLocalScoped().Use(
					TransportMock.CreateWithoutStructureMapClear(typeof (BugzillaProfile).Assembly)));
		}

		[Test]
		public void ShouldCheckValidConnection()
		{
			@"
				Given bugzilla profile created
					And Bugzilla url set to 'http://new-bugzilla/bugzilla363'
					And login set to 'bugzilla@targetprocess.com'
					And password set to 'bugzillaadmin'
					And query set to 'Test'
				When check connection to Bugzilla
				Then connection should be successful
			"
				.Execute();
		}

		[Test]
		public void ShouldCheckConnectionWithInvalidPath()
		{
			@"
				Given bugzilla profile created
					And Bugzilla url set to 'http://new-bugzilla/bugzillaSomeNumbers'
					And login set to 'bugzilla@targetprocess.com'
					And password set to 'bugzillaadmin'
				When check connection to Bugzilla
				Then connection should be failed
			"
				.Execute();
		}

		[Test]
		public void ShouldCheckConnectionWithInvalidCredentials()
		{
			@"
				Given bugzilla profile created
					And Bugzilla url set to 'http://new-bugzilla/bugzilla363'
					And login set to 'invalid@email.com'
					And password set to 'pass'
				When check connection to Bugzilla
				Then authentication should be failed
			"
				.Execute();
		}

		[Test]
		public void ShouldCheckConnectionWithInvalidQuery()
		{
			@"
				Given bugzilla profile created
					And Bugzilla url set to 'http://new-bugzilla/bugzilla363'
					And login set to 'bugzilla@targetprocess.com'
					And password set to 'bugzillaadmin'
					And query set to 'InvalidQuery'
				When check connection to Bugzilla
				Then query validation should be failed
			"
				.Execute();
		}

		[Test]
		public void ShouldCheckValidConnectionWhenQueriesSeparatedWithSpaces()
		{
			@"
				Given bugzilla profile created
					And Bugzilla url set to 'http://new-bugzilla/bugzilla363'
					And login set to 'bugzilla@targetprocess.com'
					And password set to 'bugzillaadmin'
					And query set to 'Test,  New'
				When check connection to Bugzilla
				Then connection should be successful
			"
				.Execute();
		}

		[Given("bugzilla profile created")]
		public void CreateBugzillaProfile()
		{
			Context.AddProfile("Profile_1", 1);
		}

		[Given("Bugzilla url set to '$bugzillaUrl'")]
		public void SetBugzillaPath(string bugzillaUrl)
		{
			Profile.GetProfile<BugzillaProfile>().Url = bugzillaUrl;
		}

		[Given("login set to '$login'")]
		public void SetLogin(string login)
		{
			Profile.GetProfile<BugzillaProfile>().Login = login;
		}

		[Given("password set to '$password'")]
		public void SetBugzillaPassword(string password)
		{
			Profile.GetProfile<BugzillaProfile>().Password = password;
		}

		[Given("query set to '$query'")]
		public void SetBugzillaQuery(string query)
		{
			Profile.GetProfile<BugzillaProfile>().SavedSearches = query;
		}

		[When("check connection to Bugzilla")]
		public void CheckConnection()
		{
			try
			{
				new BugzillaService().CheckConnection(Profile.GetProfile<BugzillaProfile>());
			}
			catch (BugzillaPluginProfileException e)
			{
				foreach (var error in e.ErrorCollection)
				{
					_errors.Add(error);
				}
			}
		}

		[Then("connection should be successful")]
		public void ConnectionShouldBeSuccessful()
		{
			_errors.Any().Should(Be.False);
		}

		[Then("authentication should be failed")]
		public void ConnectionShouldBeFailedForInvalidCredentials()
		{
			CheckErrorFields(new[] {BugzillaProfile.PasswordField, BugzillaProfile.LoginField});
		}

		[Then("connection should be failed")]
		public void ConnectionShouldBeFailed()
		{
			CheckErrorFields(new[] {BugzillaProfile.UrlField});
		}

		[Then("query validation should be failed")]
		public void QueryValidationShouldBeFailed()
		{
			CheckErrorFields(new[] {BugzillaProfile.QueriesField});
		}

		private void CheckErrorFields(IEnumerable<string> errorFields)
		{
			_errors.Select(e => e.FieldName).ToList().Should(Be.EquivalentTo(errorFields));
		}

		private static BugzillaProfile CreateProfile(string profileName)
		{
			return
				ObjectFactory.GetInstance<TransportMock>().AddProfile(profileName, new BugzillaProfile
				                                                                   	{
				                                                                   		Login = "login",
				                                                                   		Password = "password",
				                                                                   		Project = 2,
				                                                                   		SavedSearches = "query123",
				                                                                   		Url = "http://test/com",
				                                                                   	}).GetProfile<BugzillaProfile>();
		}
	}
}