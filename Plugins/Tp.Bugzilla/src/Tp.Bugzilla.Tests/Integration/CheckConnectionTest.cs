// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	[Category("PartPlugins0")]
	public class CheckConnectionTest : BugzillaTestBase
	{
		private readonly PluginProfileErrorCollection _errors = new PluginProfileErrorCollection();

		[SetUp]
		public void Setup()
		{
			ObjectFactory.Configure(x => x.For<TransportMock>().HybridHttpOrThreadLocalScoped().Use(TransportMock.CreateWithoutStructureMapClear(typeof (BugzillaProfile).Assembly)));
		}

		[Test]
		public void ShouldCheckValidConnection()
		{
			@"
				Given bugzilla profile created
					And Bugzilla url set to 'default'
					And login set to 'default'
					And password set to 'default'
					And query set to 'default'
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
					And login set to 'default'
					And password set to 'default'
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
					And Bugzilla url set to 'default'
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
					And Bugzilla url set to 'default'
					And login set to 'default'
					And password set to 'default'
					And query set to 'InvalidQuery'
				When check connection to Bugzilla
				Then query validation should be failed
			"
				.Execute();
		}

		[Test, Ignore("Move to functional test because of test searches presetup is needed")]
		public void ShouldCheckValidConnectionWhenQueriesSeparatedWithSpaces()
		{
			@"
				Given bugzilla profile created
					And Bugzilla url set to 'default'
					And login set to 'default'
					And password set to 'default'
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
			Profile.GetProfile<BugzillaProfile>().Url = GivenStepValueSelector.Select(bugzillaUrl, BugzillaTestConstants.Url);
		}

		[Given("login set to '$login'")]
		public void SetLogin(string login)
		{
			Profile.GetProfile<BugzillaProfile>().Login = GivenStepValueSelector.Select(login, BugzillaTestConstants.Login);
		}

		[Given("password set to '$password'")]
		public void SetBugzillaPassword(string password)
		{
			Profile.GetProfile<BugzillaProfile>().Password = GivenStepValueSelector.Select(password, BugzillaTestConstants.Password);
		}

		[Given("query set to '$query'")]
		public void SetBugzillaQuery(string query)
		{
			Profile.GetProfile<BugzillaProfile>().SavedSearches = GivenStepValueSelector.Select(query, BugzillaTestConstants.Queries);
		}

		private static class GivenStepValueSelector
		{
			public static string Select(string value, string defaultValue)
			{
				return value == "default" ? defaultValue : value;
			}
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
			var sb = _errors.Aggregate(new StringBuilder(), (acc, err) => acc.AppendLine(err.Message));
			if (sb.Length != 0)
			{
				Console.WriteLine("ConnectionShouldBeSuccessful errors:");
				Console.WriteLine(sb);
			}
			_errors.Should(Be.Empty, "_errors.Should(Be.Empty)");
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
			_errors.Select(e => e.FieldName).ToList().Should(Be.EquivalentTo(errorFields), "_errors.Select(e => e.FieldName).ToList().Should(Be.EquivalentTo(errorFields))");
		}
	}
}