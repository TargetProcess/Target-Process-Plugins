// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.Integration.Testing.Common;
using Tp.TestRunImport.Tests.Context;
using Tp.TestRunImport.Tests.StructureMap;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.TestRunImport.Tests.Commands
{
	[TestFixture, ActionSteps]
	public class ValidateProfileForMappingSpecs
	{
		private TestRunImportPluginProfile _settings;

		[BeforeScenario]
		public void BeforeScenario()
		{
			_settings = new TestRunImportPluginProfile();
			ObjectFactory.Configure(
				x =>
					x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(TestRunImportPluginProfile).Assembly)));
			ObjectFactory.Configure(x => x.AddRegistry<TestRunImportEnvironmentRegistry>());
		}

		[AfterScenario]
		public virtual void OnAfterScenario()
		{
			Context.Transport.LocalQueue.Clear();
			Context.Transport.TpQueue.Clear();
		}

		[Test]
		public void ShouldBeSuccessOnValidSettings()
		{
			@"Given Xml results file path path 'NUnit\SimpleTestCaseTestResult.xml'
				And framework type is 'NUnit'
				And projectId '11'
				And testPlanId '101'
			When profile is validated for mapping
			Then no errors should occur
			".Execute();
		}

		[Test]
		public void ShouldFailWhenNoFrameworkTypeInSettings()
		{
			@"Given Xml results file path path 'NUnit\SimpleTestCaseTestResult.xml'
				And projectId '11'
				And testPlanId '101'
			When profile is validated for mapping
			Then error should occur for FrameworkType: ""Framework type should be specified""
			".Execute();
		}

		[Given("Xml results file path path '$uri'")]
		public void GivenRepositoryPath(string uri)
		{
			_settings.ResultsFilePath = string.Format("{0}\\{1}", Environment.CurrentDirectory, uri.TrimStart('\\'));
		}

		[Given("framework type is '$type'")]
		public void GivenSynchronizationInterval(string type)
		{
			if (string.Compare("NUnit", type, StringComparison.InvariantCultureIgnoreCase) == 0)
				_settings.FrameworkType = FrameworkTypes.NUnit;
			else if (string.Compare("JUnit", type, StringComparison.InvariantCultureIgnoreCase) == 0)
				_settings.FrameworkType = FrameworkTypes.JUnit;
			else if (string.Compare("Selenium", type, StringComparison.InvariantCultureIgnoreCase) == 0)
				_settings.FrameworkType = FrameworkTypes.Selenium;
			else
				_settings.FrameworkType = FrameworkTypes.None;
		}

		[Given("projectId '$projectId'")]
		public void GivenProjectId(int projectId)
		{
			_settings.Project = projectId;
		}

		[Given("testPlanId '$testPlanId'")]
		public void GiventestPlanId(int testPlanId)
		{
			_settings.TestPlan = testPlanId;
		}

		[When("profile is validated for mapping")]
		public void WhenProfileIsValidatedForMapping()
		{
			var args = new PluginProfileDto { Settings = _settings }.Serialize();
			var command = new ExecutePluginCommandCommand { CommandName = "ValidateProfileForMapping", Arguments = args };

			Context.Transport.TpQueue.Clear();
			Context.Transport.HandleMessageFromTp(command);
		}
		
		[Then("no errors should occur")]
		public void NoErrorsShouldOccur()
		{
			// TODO: remove select when updated sdk with beautiful ToString version
			var errors = GetSuccessfullResponse().ResponseData.Deserialize<PluginProfileErrorCollection>().Select(x => x.Message).ToList();
			errors.Should(Be.Empty);
		}

		[Then(@"error should occur for $fieldName: ""$errorMessage""")]
		public void ErrorShouldOccur(string fieldName, string errorMessage)
		{
			var errors = GetSuccessfullResponse().ResponseData.Deserialize<PluginProfileErrorCollection>();
			new[] { errorMessage }.Should(Be.SubsetOf(errors.Where(x => x.FieldName == fieldName).Select(x => x.Message).ToList()));
		}

		[Then("error should occur for $fieldName")]
		public void ErrorShouldOccur(string fieldName)
		{
			var errors = GetSuccessfullResponse().ResponseData.Deserialize<PluginProfileErrorCollection>();
			errors.Where(x => x.FieldName == fieldName).Should(Be.Not.Empty);
		}

		private PluginCommandResponseMessage GetSuccessfullResponse()
		{
			var response = Context.Transport.TpQueue.GetMessages<PluginCommandResponseMessage>().Single();
			response.PluginCommandStatus.Should(Be.EqualTo(PluginCommandStatus.Succeed));
			return response;
		}

		public TestRunImportPluginContext Context
		{
			get { return ObjectFactory.GetInstance<TestRunImportPluginContext>(); }
		}
	}
}