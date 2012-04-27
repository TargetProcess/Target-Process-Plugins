// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Data.Linq;
using System.Linq;
using System.Xml;
using NBehave.Narrator.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Integration.Plugin.Common.Storage.Repositories;
using Tp.LegacyProfileConvertsion.Common;
using Tp.Testing.Common.NUnit;

namespace Tp.LegacyProfileConversion.Common.Testing
{
	[ActionSteps]
	public abstract class LegacyProfileConverterActionStepsBase<TConverter, TRegistry>
		where TConverter : LegacyProfileConvertorBase<PluginProfile>
		where TRegistry : LegacyProfileConverterUnitTestRegistry, new()
	{
		protected TpDatabaseDataContext Context { get; private set; }
		protected PluginProfile _legacyProfile;
		protected int DefaultProcessId { get; set; }

		private static void ClearPluginDb(string pluginConnectionString)
		{
			var context = new PluginDatabaseModelDataContext(pluginConnectionString);
			if (!context.DatabaseExists())
			{
				context.CreateDatabase();
				context.SubmitChanges();
			}
			else
			{
				ClearDatabase(context);
			}
		}

		private static void ClearDatabase(PluginDatabaseModelDataContext context)
		{
			var plugins = from plugin in context.Plugins select plugin;
			Array.ForEach(plugins.ToArray(), x => context.Plugins.DeleteOnSubmit(x));
			context.SubmitChanges();
		}

		public void OnDatabaseTearDown()
		{
		}

		[BeforeScenario]
		public virtual void OnBeforeScenario()
		{
			ObjectFactory.Configure(x => x.AddRegistry<TRegistry>());
			ClearPluginDb(PluginConnectionString);
			ClearTpDb(new TpDatabaseDataContext(TpConnectionString));

			Context = new TpDatabaseDataContext(TpConnectionString);
			DefaultProcessId = Context.Processes.First(p => p.IsDefault == true).ProcessID;
		}

		[AfterScenario]
		public virtual void OnAfterScenario()
		{
			ClearTpDb(Context);
			var context = new PluginDatabaseModelDataContext(PluginConnectionString);
			ClearDatabase(context);
		}

		private static void ClearTpDb(DataContext tpDatabaseDataContext)
		{
			tpDatabaseDataContext.ExecuteCommand("delete from TestPlan");
			tpDatabaseDataContext.ExecuteCommand("delete from Comment");
			tpDatabaseDataContext.ExecuteCommand("delete from Attachment");
			tpDatabaseDataContext.ExecuteCommand("delete from Bug");
			tpDatabaseDataContext.ExecuteCommand("update General set ParentProjectID=null");
			tpDatabaseDataContext.ExecuteCommand("delete from ProjectMember");
			tpDatabaseDataContext.ExecuteCommand("delete from Project");
			tpDatabaseDataContext.ExecuteCommand("delete from CustomReport");
			tpDatabaseDataContext.ExecuteCommand("delete from TpUser");
			tpDatabaseDataContext.ExecuteCommand("delete from ExternalReference");
			tpDatabaseDataContext.ExecuteCommand("delete from Assignable");
			tpDatabaseDataContext.ExecuteCommand("delete from General");
			tpDatabaseDataContext.ExecuteCommand("delete from MessageUid");
			tpDatabaseDataContext.ExecuteCommand("delete from PluginProfile");
			tpDatabaseDataContext.ExecuteCommand("delete from Revision");
		}

		[Given("account name is '$accountName'")]
		public void AccountNameIs(string accountName)
		{
			ObjectFactory.GetInstance<IConvertorArgs>().Stub(x => x.AccountName).Return(accountName);
		}

		[Given("project '$projectAbbr' created")]
		public void CreateProject(string projectAbbr)
		{
			CreateProjectForProcess(projectAbbr);
		}

		[Given("project '$projectAbbr' for the first process created")]
		public void CreateProjectForProcess(string projectAbbr)
		{
			Context.Generals.InsertOnSubmit(new General { Name = projectAbbr });
			Context.SubmitChanges();

			var project = new Project
			{
				Abbreviation = projectAbbr,
				ProjectID = Context.Generals.First(x => x.Name == projectAbbr).GeneralID,
				IsActive = true,
				ProcessID = DefaultProcessId
			};
			Context.Projects.InsertOnSubmit(project);
			Context.SubmitChanges();
		}

		[Given("test plan '$testPlanName' for project '$projectAbbr' created")]
		public void CreateTestPlanForProject(string testPlanName, string projectAbbr)
		{
			Project project = Context.Projects.First(x => x.Abbreviation == projectAbbr);
			Context.Generals.InsertOnSubmit(new General { Name = testPlanName });
			Context.SubmitChanges();
			Context.TestPlans.InsertOnSubmit(new TestPlan
			{
				TestPlanID = Context.Generals.First(x => x.Name == testPlanName).GeneralID,
				ProjectID = project.ProjectID
			});
			Context.SubmitChanges();
		}

		[Given("profile name is '$profileName'")]
		public void SetProfileName(string profileName)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(
				string.Format(
					@"<?xml version=""1.0"" encoding=""utf-16""?>
<{0} xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
</{0}>",
					SettingsXmlNode));
			_legacyProfile = new PluginProfile
			                 	{
			                 		PluginName = PluginName,
			                 		ProfileName = profileName,
			                 		Active = true,
			                 		Settings = xmlDocument.OuterXml,
			                 	};
			Context.PluginProfiles.InsertOnSubmit(_legacyProfile);
		}

		protected string GetLegacyProfileValue(string valueName)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(_legacyProfile.Settings);
			return GetNodeByName(valueName, xmlDocument).InnerText;
		}

		protected void SetLegacyProfileValue(string valueName, string value)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(_legacyProfile.Settings);
			var valueNode = GetNodeByName(valueName, xmlDocument);
			valueNode.InnerXml = value;
			xmlDocument.SelectSingleNode(string.Format("./{0}", SettingsXmlNode)).AppendChild(valueNode);

			_legacyProfile.Settings = xmlDocument.OuterXml;
		}

		private static XmlNode GetNodeByName(string valueName, XmlDocument xmlDocument)
		{
			return xmlDocument.SelectSingleNode("//" + valueName) ?? xmlDocument.CreateElement(valueName);
		}

		[When(@"legacy plugin profile from Target Process converted to new plugin profile")]
		public void ConvertPluginProfile()
		{
			Context.SubmitChanges();

			var args = ObjectFactory.GetInstance<IConvertorArgs>();
			args.Stub(x => x.TpConnectionString).Return(TpConnectionString);
			args.Stub(x => x.PluginConnectionString).Return(PluginConnectionString);
			args.Stub(x => x.ConnectionString).Return(args.PluginConnectionString);

			ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
			ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(args));
			ObjectFactory.GetInstance<TConverter>().Execute();
		}

		[When(@"legacy plugin profile from Target Process converted to new plugin profile without Plugin DB specified")]
		public void ConvertPluginProfileWithNoPluginDbSpecified()
		{
			var args = ObjectFactory.GetInstance<IConvertorArgs>();
			args.Stub(x => x.TpConnectionString).Return(TpConnectionString);
			args.Stub(x => x.PluginConnectionString).Return(args.TpConnectionString);
			args.Stub(x => x.ConnectionString).Return(args.TpConnectionString);

			ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
			ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(args));
			ObjectFactory.GetInstance<TConverter>().Execute();
		}

		[Then("plugin '$pluginName' should have account '$accountName'")]
		public void PluginShouldHaveAccount(string pluginName, string accountName)
		{
			var accounts =AccountRepository.GetAll().Where(x => x.Name == accountName).Select(y => y.Name.Value).ToArray();
			accounts.Should(Be.EquivalentTo(new[] {accountName}));
		}

		[Then("'$profileName' plugin profile should be created")]
		public void PluginShouldHaveProfile(string profileName)
		{
			PluginProfilesShouldBeCreated(new[] {profileName});
		}

		[Then(@"plugin profiles should be created: (?<pluginProfileNames>([^,]+,?\s*)+)")]
		public void PluginProfilesShouldBeCreated(string[] pluginProfileNames)
		{
			var profiles =
				(from profile in Account.Profiles select profile.Name.Value).ToArray();
			profiles.Should(Be.EquivalentTo(pluginProfileNames));
		}

		[Then("plugin profile names should be unique")]
		public void PluginProfileNamesShouldBeUnique()
		{
			var profileNames = Account.Profiles.Select(x => x.Name.Value).ToArray();
			profileNames.Distinct().ToArray().Should(Be.EquivalentTo(profileNames));
		}

		[Then("sync interval should be predefined $minutes")]
		public void SyncIntervalShouldBeSpecified(int minutes)
		{
			Account.Profiles.Cast<ISynchronizableProfile>().Single().SynchronizationInterval.Should(Be.EqualTo(minutes));
		}

		protected static IAccount Account
		{
			get
			{
				return AccountRepository.GetAll().First(x => x.Name == ObjectFactory.GetInstance<IConvertorArgs>().AccountName);
			}
		}

		private static IAccountRepository AccountRepository
		{
			get { return ObjectFactory.GetInstance<IAccountRepository>(); }
		}

		protected virtual string PluginConnectionString
		{
			get { return "Data Source=(local);Initial Catalog=TargetProcessTest;user id=sa;password=sa"; }
		}

		protected virtual string TpConnectionString
		{
			get { return "Data Source=(local);Initial Catalog=TargetProcessTest;user id=sa;password=sa"; }
		}

		protected abstract string SettingsXmlNode { get; }
		protected abstract string PluginName { get; }
	}
}