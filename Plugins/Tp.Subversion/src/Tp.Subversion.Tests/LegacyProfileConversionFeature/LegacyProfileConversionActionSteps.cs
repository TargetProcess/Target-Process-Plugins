// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Xml;
using NBehave.Narrator.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.LegacyProfileConvertsion.Common;
using Tp.SourceControl.RevisionStorage;
using Tp.Subversion.LegacyProfileConversion;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.LegacyProfileConversionFeature
{
	[ActionSteps]
	public class LegacyProfileConverterActionSteps
	{
		private TpDatabaseDataContext _context;
		private PluginProfile _legacyProfile;

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

		public void OnDatabaseTearDown() {}

		[BeforeScenario]
		public void OnBeforeScenario()
		{
			ObjectFactory.Configure(x => x.AddRegistry<SubversionLegacyProfileConversionUnitTestRegistry>());
			ClearPluginDb(Properties.Settings.Default.PluginConnectionString);
			ClearTpDB(new TpDatabaseDataContext(Properties.Settings.Default.TpConnectionString));

			_context = new TpDatabaseDataContext(Properties.Settings.Default.TpConnectionString);
		}

		[AfterScenario]
		public void OnAfterScenario()
		{
			ClearTpDB(_context);
			var context = new PluginDatabaseModelDataContext(Properties.Settings.Default.PluginConnectionString);
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
			tpDatabaseDataContext.ExecuteCommand("delete from Revision");
		}

		[Given("account name is '$accountName'")]
		public void AccountNameIs(string accountName)
		{
			ObjectFactory.GetInstance<IConvertorArgs>().Stub(x => x.AccountName).Return(accountName);
		}

		[Given("profile name is '$profileName'")]
		public void SetProfileName(string profileName)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(
				@"<?xml version=""1.0"" encoding=""utf-16""?>
<Settings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
</Settings>");
			_legacyProfile = new PluginProfile
			{
				PluginName = "Subversion Integration",
				ProfileName = profileName,
				Active = true,
				Settings = xmlDocument.OuterXml,
			};
			_context.PluginProfiles.InsertOnSubmit(_legacyProfile);
		}

		[Given("the last imported revision is $revision")]
		public void SetLastImportedRevision(int revision)
		{
			_context.SubmitChanges();

			var lastRevision = new Revision {PluginProfileID = _legacyProfile.PluginProfileID, SourceControlID = revision, CommitDate = DateTime.Today.AddDays(-20)};
			_context.Revisions.InsertOnSubmit(lastRevision);

			ConvertEmailPluginProfile();
		}

		[Given("subversion repository is '$subversionRepoUrl'")]
		public void SetSubversionRepo(string subversionRepoUrl)
		{
			SetLegacyProfileValue("PathToProject", subversionRepoUrl);
		}

		[Given("sync interval is $syncInterval")]
		public void SetSyncInterval(int syncInterval)
		{
			SetLegacyProfileValue("SyncInterval", syncInterval.ToString());
		}

		[Given("subversion login is '$login'")]
		public void SetSvnLogin(string login)
		{
			SetLegacyProfileValue("SubversionLogin", login);
		}

		[Given("subversion password is '$password'")]
		public void SetSvnPassword(string password)
		{
			SetLegacyProfileValue("SubversionPassword", password);
		}

		[Given("subversion starting revision is $revision")]
		public void SetSvnStartingRevision(int revision)
		{
			SetLegacyProfileValue("StartRevision", revision.ToString());
		}

		[Given("user mapping is:")]
		public void UpdateUserMapping(string subversion, string targetprocess)
		{
			var maps = GetLegacyProfileValue("Maps");
			var settings = new LegacyMappingParser();
			if (!string.IsNullOrEmpty(maps))
			{
				settings.Maps = maps.TrimEnd(Environment.NewLine.ToArray());
			}

			settings.Users[subversion] = targetprocess;

			SetLegacyProfileValue("Maps", settings.Maps);
		}

		[Given("subversion revision $revisionId is imported")]
		public void ImportRevision(int revisionId)
		{
			_context.Revisions.InsertOnSubmit(new Revision
			                                  	{
			                                  		SourceControlID = revisionId,
													CommitDate = CurrentDate.Value,
													PluginProfileID = _legacyProfile.PluginProfileID
			                                  	});
			_context.SubmitChanges();
		}

		private string GetLegacyProfileValue(string valueName)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(_legacyProfile.Settings);
			return GetNodeByName(valueName, xmlDocument).InnerText;
		}

		private void SetLegacyProfileValue(string valueName, string value)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(_legacyProfile.Settings);
			var valueNode = GetNodeByName(valueName, xmlDocument);
			valueNode.InnerXml = value;
			xmlDocument.SelectSingleNode("./Settings").AppendChild(valueNode);

			_legacyProfile.Settings = xmlDocument.OuterXml;
		}

		private static XmlNode GetNodeByName(string valueName, XmlDocument xmlDocument)
		{
			return xmlDocument.SelectSingleNode("//" + valueName) ?? xmlDocument.CreateElement(valueName);
		}

		[When(@"legacy subversion plugin profile from Target Process converted to new subversion plugin profile")]
		public void ConvertEmailPluginProfile()
		{
			_context.SubmitChanges();

			var args = ObjectFactory.GetInstance<IConvertorArgs>();
			args.Stub(x => x.TpConnectionString).Return(Properties.Settings.Default.TpConnectionString);
			args.Stub(x => x.PluginConnectionString).Return(Properties.Settings.Default.PluginConnectionString);
			args.Stub(x => x.ConnectionString).Return(args.PluginConnectionString);

			ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
			ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(args));
			ObjectFactory.GetInstance<LegacyProfileConvertor>().Execute();
		}

		[When(@"revisions migrated from old profile to the new one")]
		public void MigrateRevisionsFromOldProfileToTheNewOne()
		{
			_context.SubmitChanges();

			var args = ObjectFactory.GetInstance<IConvertorArgs>();
			args.Stub(x => x.TpConnectionString).Return(Properties.Settings.Default.TpConnectionString);
			args.Stub(x => x.PluginConnectionString).Return(Properties.Settings.Default.PluginConnectionString);
			args.Stub(x => x.ConnectionString).Return(args.PluginConnectionString);
			args.Stub(x => x.Action).Return("revisions");

			ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
			ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(args));
			ObjectFactory.GetInstance<LegacyRevisionsImporter>().Execute();
		}

		[Then("plugin '$pluginName' should have account '$accountName'")]
		public void PluginShouldHaveAccount(string pluginName, string accountName)
		{
			var accounts = AccountCollection.Where(x => x.Name == accountName).Select(y => y.Name.Value).ToArray();
			accounts.Should(Be.EquivalentTo(new[] {accountName}));
		}

		[Then(@"profile should have tp users: (?<users>([^,]+,?\s*)+)")]
		public void ProfileShouldHaveTpUsers(string[] users)
		{
			Storage.Get<UserDTO>().Select(x => x.Login).ToArray().Should(Be.EquivalentTo(users));
		}

		[Then(@"profile should be initialized")]
		public void ProfileShouldBeInitialized()
		{
			Storage.Initialized.Should(Be.True);
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

		[Given("requester '$requesterLogin' created ")]
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

		[Given("tp user with login '$userLogin' and email '$userMail'")]
		public void CreateUserWithMail(string userLogin, string userMail)
		{
			_context.TpUsers.InsertOnSubmit(new TpUser
			{
				Login = userLogin,
				Email = userMail,
				FirstName = "FirstName",
				LastName = "LastName",
				SecretWord = "abc",
				Type = 1
			});
			_context.SubmitChanges();
		}

		[Given("deleted tp user with login '$userLogin' and email '$userMail'")]
		public void CreateDeletedUserWithMail(string userLogin, string userMail)
		{
			_context.TpUsers.InsertOnSubmit(new TpUser
			{
				Login = userLogin,
				Email = userMail,
				FirstName = "FirstName",
				LastName = "LastName",
				SecretWord = "abc",
				Type = 1,
				DeleteDate = CurrentDate.Value
			});
			_context.SubmitChanges();
		}

		[Then("subversion repository should be '$svnUrl'")]
		public void SvnRepoUrlShouldBe(string svnUrl)
		{
			Profile.Uri.Should(Be.EqualTo(svnUrl));
		}

		[Then("sync interval should be predefined as $syncInterval")]
		public void SyncIntervalShouldBe(int syncInterval)
		{
			Profile.SynchronizationInterval.Should(Be.EqualTo(syncInterval));
		}

		[Then("subversion login should be '$login'")]
		public void SvnLoginShouldBe(string login)
		{
			Profile.Login.Should(Be.EqualTo(login));
		}

		[Then("subversion password should be '$password'")]
		public void SvnPasswordShouldBe(string password)
		{
			Profile.Password.Should(Be.EqualTo(password));
		}

		[Then("subversion starting revision should be $revision")]
		public void SvnRevisionShouldBe(int revision)
		{
			Profile.StartRevision.Should(Be.EqualTo(revision.ToString()));
		}

		[Then("'$profileName' plugin profile should be created")]
		public void PluginShouldHaveProfile(string profileName)
		{
			PluginProfilesShouldBeCreated(new[]{profileName});
		}

		[Then(@"plugin profiles should be created: (?<pluginProfileNames>([^,]+,?\s*)+)")]
		public void PluginProfilesShouldBeCreated(string[] pluginProfileNames)
		{
			var profiles =
				(from profile in Account.Profiles select profile.Name.Value).ToArray();
			profiles.Should(Be.EquivalentTo(pluginProfileNames));
		}

		[Then("$profileAmount plugin profiles should be created")]
		public void PluginShouldHaveProfilesAmountConverted(int profileAmount)
		{
			var profiles =
				(from profile in Account.Profiles select profile.GetProfile<SubversionPluginProfile>()).ToArray();
			profiles.Length.Should(Be.EqualTo(profileAmount));
		}

		[Then("user mapping should be:")]
		public void UserMappingShouldBe(string subversion, string targetprocess)
		{
			Profile.UserMapping[subversion.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()));
		}

		[Then("$userCount user snould be mapped")]
		public void UsersShouldBeMapped(int userCount)
		{
			Profile.UserMapping.Count.Should(Be.EqualTo(userCount));
		}

		[Then("plugin profile names should be unique")]
		public void PluginProfileNamesShouldBeUnique()
		{
			var profileNames = Account.Profiles.Select(x => x.Name.Value).ToArray();
			profileNames.Distinct().ToArray().Should(Be.EquivalentTo(profileNames));
		}

		[Then(@"profile should have revisions: (?<revisionIds>([^,]+,?\s*)+)")]
		public void PluginProfileShouldContainRevisions(int[] revisionIds)
		{
			Storage.Get<RevisionIdRelation>().Count(x => revisionIds.Contains(Int32.Parse(x.RevisionId))).Should(Be.EqualTo(revisionIds.Length));
		}

		private static SubversionPluginProfile Profile
		{
			get { return Storage.GetProfile<SubversionPluginProfile>(); }
		}

		private static IProfileReadonly Storage
		{
			get { return Account.Profiles.First(); }
		}

		private static IAccountReadonly Account
		{
			get
			{
				var accountName = ObjectFactory.GetInstance<IConvertorArgs>().AccountName;
				return AccountCollection.First(acc => acc.Name == accountName);
			}
		}

		private static IEnumerable<IAccountReadonly> AccountCollection
		{
			get { return ObjectFactory.GetInstance<IAccountCollection>(); }
		}

	}
}
