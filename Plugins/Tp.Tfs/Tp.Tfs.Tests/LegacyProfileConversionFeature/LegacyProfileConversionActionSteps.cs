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
using Tp.LegacyProfileConversion.Common.Testing;
using Tp.LegacyProfileConvertsion.Common;
using Tp.SourceControl;
using Tp.SourceControl.RevisionStorage;
using Tp.Tfs.LegacyProfileConversion;
using Tp.Testing.Common.NUnit;

namespace Tp.Tfs.Tests.LegacyProfileConversionFeature
{
    [ActionSteps]
    public class LegacyProfileConverterActionSteps
        : LegacyProfileConverterActionStepsBase
            <LegacyProfileConvertor, TfsLegacyProfileConversionUnitTestRegistry, PluginProfile>
    {
        [Given("the last imported revision is $revision")]
        public void SetLastImportedRevision(int revision)
        {
            Context.SubmitChanges();

            var lastRevision = new Revision
            {
                PluginProfileID = LegacyProfile.PluginProfileID,
                SourceControlID = revision,
                CommitDate = DateTime.Today.AddDays(-20)
            };
            Context.Revisions.InsertOnSubmit(lastRevision);

            ConvertPluginProfile();
        }

        [Given("tfs server uri is '$tfsServerUrl'")]
        public void SetTfsServerUri(string tfsServerUrl)
        {
            SetLegacyProfileValue("ServerName", tfsServerUrl);
        }

        [Given("team project name is '$teamProjectName'")]
        public void SetTeamProjectName(string teamProjectName)
        {
            SetLegacyProfileValue("TeamProjectName", teamProjectName);
        }

        [Given("sync interval is $syncInterval")]
        public void SetSyncInterval(int syncInterval)
        {
            SetLegacyProfileValue("SyncInterval", syncInterval.ToString());
        }

        [Given("tfs login is '$login'")]
        public void SetTfsLogin(string login)
        {
            SetLegacyProfileValue("Login", login);
        }

        [Given("tfs password is '$password'")]
        public void SetTfsPassword(string password)
        {
            SetLegacyProfileValue("Password", password);
        }

        [Given("tfs starting revision is $revision")]
        public void SetTfsStartingRevision(int revision)
        {
            SetLegacyProfileValue("StartRevision", revision.ToString());
        }

        [Given("user mapping is:")]
        public void UpdateUserMapping(string tfs, string targetprocess)
        {
            var maps = GetLegacyProfileValue("Maps");
            var settings = new LegacyMappingParser();
            if (!string.IsNullOrEmpty(maps))
            {
                settings.Maps = maps.TrimEnd(Environment.NewLine.ToArray());
            }

            settings.Users[tfs] = targetprocess;

            SetLegacyProfileValue("Maps", settings.Maps);
        }

        [Given("tfs revision $revisionId is imported")]
        public void ImportRevision(int revisionId)
        {
            Context.Revisions.InsertOnSubmit(new Revision
            {
                SourceControlID = revisionId,
                CommitDate = CurrentDate.Value,
                PluginProfileID = LegacyProfile.PluginProfileID
            });
            Context.SubmitChanges();
        }

        [When(@"legacy tfs plugin profile from Target Process converted to new tfs plugin profile")]
        public void ConvertTfsPluginProfile()
        {
            Context.SubmitChanges();

            var args = ObjectFactory.GetInstance<IConvertorArgs>();
            args.Stub(x => x.TpConnectionString).Return(Properties.Settings.Default.TpConnectionString);
            args.Stub(x => x.PluginConnectionString).Return(Properties.Settings.Default.PluginConnectionString);
            args.Stub(x => x.ConnectionString).Return(args.PluginConnectionString);

            ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
            ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(args));
            ObjectFactory.GetInstance<LegacyProfileConvertor>().Execute(LegacyProfile.ProfileName);
        }

        [When(@"revisions migrated from old profile to the new one")]
        public void MigrateRevisionsFromOldProfileToTheNewOne()
        {
            Context.SubmitChanges();

            var args = ObjectFactory.GetInstance<IConvertorArgs>();
            args.Stub(x => x.TpConnectionString).Return(Properties.Settings.Default.TpConnectionString);
            args.Stub(x => x.PluginConnectionString).Return(Properties.Settings.Default.PluginConnectionString);
            args.Stub(x => x.ConnectionString).Return(args.PluginConnectionString);
            args.Stub(x => x.Action).Return("revisions");

            ObjectFactory.EjectAllInstancesOf<IDatabaseConfiguration>();
            ObjectFactory.Configure(x => x.For<IDatabaseConfiguration>().Use(args));
            ObjectFactory.GetInstance<LegacyRevisionsImporter>().Execute(LegacyProfile.ProfileName);
        }

        [Then(@"profile should have tp users: (?<users>([^,]+,?\s*)+)")]
        public void ProfileShouldHaveTpUsers(string[] users)
        {
            Storage.Get<TpUserData>()
                .Select(x => x.Login)
                .ToArray()
                .Should(Be.EquivalentTo(users), "Storage.Get<TpUserData>().Select(x => x.Login).ToArray().Should(Be.EquivalentTo(users))");
        }

        [Then(@"profile should be initialized")]
        public void ProfileShouldBeInitialized()
        {
            Storage.Initialized.Should(Be.True, "Storage.Initialized.Should(Be.True)");
        }

        [Given("user '$userLogin' created")]
        public void CreateUser(string userLogin)
        {
            Context.TpUsers.InsertOnSubmit(new TpUser
            {
                Login = userLogin,
                Email = string.Format("{0}@targetprocess.com", userLogin),
                FirstName = "FirstName",
                LastName = "LastName",
                SecretWord = "abc",
                Type = 1
            });
            Context.SubmitChanges();
        }

        [Given("requester '$requesterLogin' created ")]
        public void CreateRequester(string requesterLogin)
        {
            Context.TpUsers.InsertOnSubmit(new TpUser
            {
                Login = requesterLogin,
                Email = string.Format("{0}@targetprocess.com", requesterLogin),
                FirstName = "FirstName",
                LastName = "LastName",
                SecretWord = "abc",
                Type = 4
            });
            Context.SubmitChanges();
        }

        [Given("tp user with login '$userLogin' and email '$userMail'")]
        public void CreateUserWithMail(string userLogin, string userMail)
        {
            Context.TpUsers.InsertOnSubmit(new TpUser
            {
                Login = userLogin,
                Email = userMail,
                FirstName = "FirstName",
                LastName = "LastName",
                SecretWord = "abc",
                Type = 1
            });
            Context.SubmitChanges();
        }

        [Given("deleted tp user with login '$userLogin' and email '$userMail'")]
        public void CreateDeletedUserWithMail(string userLogin, string userMail)
        {
            Context.TpUsers.InsertOnSubmit(new TpUser
            {
                Login = userLogin,
                Email = userMail,
                FirstName = "FirstName",
                LastName = "LastName",
                SecretWord = "abc",
                Type = 1,
                DeleteDate = CurrentDate.Value
            });
            Context.SubmitChanges();
        }

        [Then("tfs repository should be '$tfsUrl'")]
        public void TfsRepoUrlShouldBe(string tfsUrl)
        {
            Profile.Uri.Should(Be.EqualTo(tfsUrl), "Profile.Uri.Should(Be.EqualTo(tfsUrl))");
        }

        [Then("sync interval should be predefined as $syncInterval")]
        public void SyncIntervalShouldBe(int syncInterval)
        {
            Profile.SynchronizationInterval.Should(Be.EqualTo(syncInterval),
                "Profile.SynchronizationInterval.Should(Be.EqualTo(syncInterval))");
        }

        [Then("tfs login should be '$login'")]
        public void TfsLoginShouldBe(string login)
        {
            Profile.Login.Should(Be.EqualTo(login), "Profile.Login.Should(Be.EqualTo(login))");
        }

        [Then("tfs password should be '$password'")]
        public void TfsPasswordShouldBe(string password)
        {
            Profile.Password.Should(Be.EqualTo(password), "Profile.Password.Should(Be.EqualTo(password))");
        }

        [Then("tfs starting revision should be $revision")]
        public void TfsRevisionShouldBe(int revision)
        {
            Profile.StartRevision.Should(Be.EqualTo(revision.ToString()), "Profile.StartRevision.Should(Be.EqualTo(revision.ToString()))");
        }

        [Then("$profileAmount plugin profiles should be created")]
        public void PluginShouldHaveProfilesAmountConverted(int profileAmount)
        {
            var profiles =
                (from profile in Account.Profiles select profile.GetProfile<TfsPluginProfile>()).ToArray();
            profiles.Length.Should(Be.EqualTo(profileAmount), "profiles.Length.Should(Be.EqualTo(profileAmount))");
        }

        [Then("user mapping should be:")]
        public void UserMappingShouldBe(string tfs, string targetprocess)
        {
            Profile.UserMapping[tfs.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()),
                "Profile.UserMapping[tfs.Trim()].Name.Should(Be.EqualTo(targetprocess.Trim()))");
        }

        [Then("$userCount user snould be mapped")]
        public void UsersShouldBeMapped(int userCount)
        {
            Profile.UserMapping.Count.Should(Be.EqualTo(userCount), "Profile.UserMapping.Count.Should(Be.EqualTo(userCount))");
        }

        protected override string SettingsXmlNode
        {
            get { return "Settings"; }
        }

        protected override string PluginName
        {
            get { return "Team Foundation Server Integration"; }
        }

        [Then(@"profile should have revisions: (?<revisionIds>([^,]+,?\s*)+)")]
        public void PluginProfileShouldContainRevisions(int[] revisionIds)
        {
            Storage.Get<RevisionIdRelation>()
                .Count(x => revisionIds.Contains(Int32.Parse(x.RevisionId)))
                .Should(Be.EqualTo(revisionIds.Length),
                    "Storage.Get<RevisionIdRelation>().Count(x => revisionIds.Contains(Int32.Parse(x.RevisionId))).Should(Be.EqualTo(revisionIds.Length))");
        }

        private static TfsPluginProfile Profile
        {
            get { return Storage.GetProfile<TfsPluginProfile>(); }
        }

        private static IProfileReadonly Storage
        {
            get { return Account.Profiles.First(); }
        }
    }
}
