// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Xml;
using NBehave.Narrator.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.LegacyProfileConvertsion.Common;
using Tp.PopEmailIntegration.Data;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration.LegacyProfileConversion
{
	[ActionSteps]
	public class LegacyProfileConverterActionSteps
	{
		private TpDatabaseDataContext _context;

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

		[BeforeScenario]
		public void OnBeforeScenario()
		{
			ObjectFactory.Configure(x => x.AddRegistry<PopEmailLegacyProfileConverterUnitTestRegistry>());
			ClearPluginDb(Settings.Default.PluginConnectionString);
			ClearTpDB(new TpDatabaseDataContext(Settings.Default.TpConnectionString));

			_context = new TpDatabaseDataContext(Settings.Default.TpConnectionString);
		}

		[AfterScenario]
		public void OnAfterScenario()
		{
			ClearTpDB(_context);
			var context = new PluginDatabaseModelDataContext(Settings.Default.PluginConnectionString);
			ClearDatabase(context);
		}

		private static void ClearTpDB(DataContext tpDatabaseDataContext)
		{
			tpDatabaseDataContext.ExecuteCommand("delete from Project");
			tpDatabaseDataContext.ExecuteCommand("delete from CustomReport");
			tpDatabaseDataContext.ExecuteCommand("delete from TpUser");
			tpDatabaseDataContext.ExecuteCommand("delete from General");
			tpDatabaseDataContext.ExecuteCommand("delete from MessageUid");
			tpDatabaseDataContext.ExecuteCommand("delete from PluginProfile");
		}

		[Given("project '$projectAbbr' created")]
		public void CreateProject(string projectAbbr)
		{
			_context.Generals.InsertOnSubmit(new General {Name = projectAbbr});
			_context.SubmitChanges();

			var project = new Project
			              	{
			              		Abbreviation = projectAbbr,
			              		ProjectID = _context.Generals.First(x => x.Name == projectAbbr).GeneralID,
			              		IsActive = true
			              	};
			_context.Projects.InsertOnSubmit(project);
			_context.SubmitChanges();
		}

		[Given("project '$projectAbbr' has email integration disabled")]
		public void DisableProjectEmailIntegration(string projectAbbr)
		{
			var project = _context.Projects.First(x => x.Abbreviation == projectAbbr);
			project.IsInboundMailEnabled = false;
			_context.SubmitChanges();
		}

		[Given("account name is '$accountName'")]
		public void AccountNameIs(string accountName)
		{
			ObjectFactory.GetInstance<IConvertorArgs>().Stub(x => x.AccountName).Return(accountName);
		}

		[Given("project '$projectAbbr' InboundMailCreateRequests email setting is set to $value")]
		public void SetInboundMailCreateRequestsValue(string projectAbbr, bool value)
		{
			SetProjectValue(projectAbbr, x => x.InboundMailCreateRequests = value);
		}

		[Given("user '$userLogin' created")]
		public void CreateUser(string userLogin)
		{
			_context.TpUsers.InsertOnSubmit(new TpUser
			                                	{
			                                		Login = userLogin,
			                                		Email = string.Format("{0}@targetprocess.com", userLogin),
			                                		FirstName = "FirstName",
			                                		LastName = "LastName",
			                                		SecretWord = "abc",
			                                		Type = 1
			                                	});
			_context.SubmitChanges();
		}

		[Given("requester '$requesterLogin' created")]
		public void CreateRequester(string requesterLogin)
		{
			_context.TpUsers.InsertOnSubmit(new TpUser
			                                	{
			                                		Login = requesterLogin,
			                                		Email = string.Format("{0}@targetprocess.com", requesterLogin),
			                                		FirstName = "FirstName",
			                                		LastName = "LastName",
			                                		SecretWord = "abc",
			                                		Type = 4
			                                	});
			_context.SubmitChanges();
		}

		[Given("Message with uid '$uid' for server '$server' and login '$login' exists in tp")]
		public void CreateMessageUid(string uid, string server, string login)
		{
			_context.MessageUids.InsertOnSubmit(new MessageUid {UID = uid, MailServer = server, MailLogin = login});
			_context.SubmitChanges();
		}

		[Given("project '$projectName' InboundMailAutomaticalEmailCheckTime email setting is set to $value")]
		public void SetInboundMailAutomaticalEmailCheckTime(string projectName, int value)
		{
			SetProjectValue(projectName, x => x.InboundMailAutomaticalEmailCheckTime = value);
		}

		[Given("project '$projectName' IsInboundMailEnabled email setting is set to $value")]
		public void SetIsInboundMailEnabled(string projectName, bool value)
		{
			SetProjectValue(projectName, x => x.IsInboundMailEnabled = value);
		}

		[Given("project '$projectName' InboundMailReplyAddress is set to '$value'")]
		public void SetInboundMailReplyAddress(string projectName, string value)
		{
			SetProjectValue(projectName, x => x.InboundMailReplyAddress = value);
		}

		[Given("project '$projectName' InboundMailAutoCheck is set to $value")]
		public void SetInboundMailAutoCheck(string projectName, bool value)
		{
			SetProjectValue(projectName, x => x.InboundMailAutoCheck = value);
		}

		[Given("project '$projectName' InboundMailServer is set to '$value'")]
		public void SetInboundMailServer(string projectName, string value)
		{
			SetProjectValue(projectName, x => x.InboundMailServer = value);
		}

		[Given("project '$projectName' InboundMailPort is set to $value")]
		public void SetInboundMailPort(string projectName, int value)
		{
			SetProjectValue(projectName, x => x.InboundMailPort = value);
		}

		[Given("project '$projectName' InboundMailUseSSL is set to $value")]
		public void SetInboundMailUseSSL(string projectName, bool value)
		{
			SetProjectValue(projectName, x => x.InboundMailUseSSL = value);
		}

		[Given("project '$projectName' InboundMailLogin is set to '$value'")]
		public void SetInboundMailLogin(string projectName, string value)
		{
			SetProjectValue(projectName, x => x.InboundMailLogin = value);
		}

		[Given("project '$projectName' InboundMailPassword is set to '$value'")]
		public void SetInboundMailPassword(string projectName, string value)
		{
			SetProjectValue(projectName, x => x.InboundMailPassword = value);
		}

		[Given("project '$projectName' InboundMailProtocol is set to '$value'")]
		public void SetInboundMailProtocol(string projectName, string value)
		{
			SetProjectValue(projectName, x => x.InboundMailProtocol = value);
		}

		[Given("bind email plugin has active profile '$profileName'")]
		public void BindEmailPluginHasActiveProfile(string profileName)
		{
			CreateProfile(profileName, true);
		}

		[Given("bind email plugin has inactive profile '$profileName'")]
		public void BindEmailPluginHasInactiveProfile(string profileName)
		{
			CreateProfile(profileName, false);
		}

		private void CreateProfile(string profileName, bool isActive)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(
				@"<?xml version=""1.0"" encoding=""utf-16""?>
<Settings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
	<Map></Map>
</Settings>");
			_context.PluginProfiles.InsertOnSubmit(new PluginProfile
			                                       	{
			                                       		PluginName = "Bind Email/Request To Project",
			                                       		ProfileName = profileName,
			                                       		Active = isActive,
			                                       		Settings = xmlDocument.OuterXml
			                                       	});
			_context.SubmitChanges();
		}

		[Given("bind email plugin profile '$profileName' has key '$projectAbbr' and value '$value'")]
		public void AddKeyToProfile(string profileName, string projectAbbr, string value)
		{
			var profile = _context.PluginProfiles.First(x => x.ProfileName == profileName);
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(profile.Settings);
			var mapNode = xmlDocument.SelectSingleNode("/Settings/Map");

			var projecId = _context.Projects.First(x => x.Abbreviation == projectAbbr).ProjectID;
			var map = new StringDictionary {{projecId.ToString(), value}};
			mapNode.InnerText += WriteMap(map);

			profile.Settings = xmlDocument.OuterXml;

			_context.SubmitChanges();
		}

		private static string WriteMap(StringDictionary map)
		{
			var builder = new StringBuilder();
			foreach (DictionaryEntry keyValuePair in map)
			{
				builder.AppendFormat("{0}_&_{1}_$_", keyValuePair.Key, keyValuePair.Value);
			}

			return builder.ToString();
		}

		private void SetProjectValue(string projectName, Action<Project> changeProjectAction)
		{
			var project = _context.Projects.First(x => x.Abbreviation == projectName);
			changeProjectAction(project);
			_context.SubmitChanges();
		}

		[When(@"email settings from Target Process converted to e-mail plugin profile")]
		public void ConvertProjectSettingsToEmailPluginProfile()
		{
			var args = ObjectFactory.GetInstance<IConvertorArgs>();
			args.Stub(x => x.TpConnectionString).Return(Settings.Default.TpConnectionString);
			args.Stub(x => x.PluginConnectionString).Return(Settings.Default.PluginConnectionString);
			args.Stub(x => x.ConnectionString).Return(args.PluginConnectionString);

			ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
			ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(args));
			ObjectFactory.GetInstance<LegacyProfileConvertor>().Execute();
		}

		[When(@"email settings from Target Process converted to e-mail plugin profile without Plugin DB specified")]
		public void ConvertProjectSettingsToEmailPluginProfileWithNoPluginDbSpecified()
		{
			var args = ObjectFactory.GetInstance<IConvertorArgs>();
			args.Stub(x => x.TpConnectionString).Return(Settings.Default.TpConnectionString);
			args.Stub(x => x.PluginConnectionString).Return(args.TpConnectionString);
			args.Stub(x => x.ConnectionString).Return(args.TpConnectionString);

			ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
			ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(args));
			ObjectFactory.GetInstance<LegacyProfileConvertor>().Execute();
		}

		[Then("plugin '$pluginName' should have account '$accountName'")]
		public void PluginShouldHaveAccount(string pluginName, string accountName)
		{
			var accounts = AccountCollection.Where(x => x.Name == accountName).Select(y => y.Name.Value).ToArray();
			accounts.Should(Be.EquivalentTo(new[] {accountName}));
		}

		[Then("InboundMailProtocol email plugin profile should be '$protocol'")]
		public void AssertInboundMailProtocol(string protocol)
		{
			Profile.Protocol.Should(Be.EqualTo(protocol));
		}

		[Then("InboundMailServer email plugin profile should be '$server'")]
		public void AssertInboundMailServer(string server)
		{
			Profile.MailServer.Should(Be.EqualTo(server));
		}

		[Then("InboundMailLogin email plugin profile should be '$login'")]
		public void AssertInboundMailLogin(string login)
		{
			Profile.Login.Should(Be.EqualTo(login));
		}

		[Then("InboundMailPort email plugin profile should be $port")]
		public void AssertInboundMailPort(int port)
		{
			Profile.Port.Should(Be.EqualTo(port));
		}

		[Then("InboundMailUseSSL email plugin profile should be $useSsl")]
		public void AssertInboundMailUseSSL(bool useSsl)
		{
			Profile.UseSSL.Should(Be.EqualTo(useSsl));
		}

		[Then("InboundMailPassword email plugin profile should be '$password'")]
		public void AssertInboundMailPassword(string password)
		{
			Profile.Password.Should(Be.EqualTo(password));
		}

		[Then("SyncInterval email plugin profile should be $interval'")]
		public void AssertSyncInterval(int interval)
		{
			Profile.SynchronizationInterval.Should(Be.EqualTo(interval));
		}

		[Then("$profileAmount plugin profile should be created")]
		public void ProjectShouldHaveProfilesAmountConverted(int profileAmount)
		{
			var profiles =
				(from profile in Account.Profiles select profile.GetProfile<ProjectEmailProfile>()).ToArray();
			profiles.Length.Should(Be.EqualTo(profileAmount));
		}

		[Then("plugin profile names should be unique")]
		public void PluginProfileNamesShouldBeUnique()
		{
			var profileNames = Account.Profiles.Select(x => x.Name.Value).ToArray();
			profileNames.Distinct().ToArray().Should(Be.EquivalentTo(profileNames));
		}

		[Then("profile storage should contain user '$userLogin'")]
		public void ProfileStorageShouldContainUser(string userLogin)
		{
			Account.Profiles.First().Get<UserLite>().First(x => x.Login == userLogin && x.UserType == UserType.User).Should(
				Be.Not.Null);
		}

		[Then("profile storage should contain requester '$requesterLogin'")]
		public void ProfileStorageShouldContainRequester(string requesterLogin)
		{
			Account.Profiles.First().Get<UserLite>().First(x => x.Login == requesterLogin && x.UserType == UserType.Requester).
				Should(Be.Not.Null);
		}

		[Then("profile storage should contain project '$projectAbbr'")]
		public void ProfileStorageShouldContainProject(string projectAbbr)
		{
			Account.Profiles.First().Get<ProjectDTO>().First(x => x.Abbreviation == projectAbbr).Should(Be.Not.Null);
		}

		[Then("profile storage should contain messageUid with uid '$uid' for server '$server' and login '$login'")]
		public void ProfileStorageShouldContainUid(string uid, string server, string login)
		{
			Account.Profiles.First().Get<MessageUidCollection>().First().Where(x => x == uid).Should(Be.Not.Null);
		}

		[Then("profile storage should not contain messageUid with uid '$uid'")]
		public void ProfileStorageShouldNotContainUid(string uid)
		{
			Account.Profiles.First().Get<MessageUidDTO>().Any(x => x.UID == uid).Should(Be.False);
		}

		[Then("email plugin profile should have exact rule : '$rule' where ProjectId is id of project '$projectAbbr'")]
		public void EmailPluginProfileShouldHaveRule(string rule, string projectAbbr)
		{
			var projectId = _context.Projects.First(x => x.Abbreviation == projectAbbr).ProjectID;
			Profile.Rules.Should(Be.EqualTo(rule.Replace("ProjectId", projectId.ToString())));
		}

		[Then("email plugin profile should contains rule : '$rule' where ProjectId is id of project '$projectAbbr'")]
		public void EmailPluginProfileShouldContainsRule(string rule, string projectAbbr)
		{
			var projectId = _context.Projects.First(x => x.Abbreviation == projectAbbr).ProjectID;
			Profile.Rules.Should(Be.StringContaining(rule.Replace("ProjectId", projectId.ToString())));
		}

		private static ProjectEmailProfile Profile
		{
			get { return Account.Profiles.First().GetProfile<ProjectEmailProfile>(); }
		}

		private static IAccount Account
		{
			get { return AccountCollection.GetOrCreate(ObjectFactory.GetInstance<IConvertorArgs>().AccountName); }
		}

		private static IAccountCollection AccountCollection
		{
			get { return ObjectFactory.GetInstance<IAccountCollection>(); }
		}
	}
}
