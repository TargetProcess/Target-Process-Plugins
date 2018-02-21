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
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Testing.Common;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Initialization;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration.BusinessScenarios
{
    [TestFixture, ActionSteps]
    [Category("PartPlugins1")]
    public class InitializationSagaSpecs
    {
        private readonly List<MessageUidDTO> _uids = new List<MessageUidDTO>();
        private int _queryCount;

        [SetUp]
        public void Setup()
        {
            TransportMock.CreateWithoutStructureMapClear(typeof(ProjectEmailProfile).Assembly);
        }

        [Test]
        public void ShouldLoadMessageUidsUponStart()
        {
            @"
				Given TargetProcess contains UID '1' for server 'pop1' and login 'login1'
					And TargetProcess contains UID '2' for server 'pop2' and login 'login2'
				When profile 'Profile1' for server 'pop1' and login 'login1' added
				Then profile should contain UID '1'
					And profile should not contain UID '2'
				"
                .Execute();
        }

        [Test]
        public void ShouldReloadUidsAfterProfileWasUpdated()
        {
            @"
				Given TargetProcess contains UID '1' for server 'pop1' and login 'login1'
					And TargetProcess contains UID '2' for server 'pop2' and login 'login2'
					And profile 'Profile1' for server 'pop1' and login 'login1' added
				When profile 'Profile1' server set to 'pop2' and login to 'login2'
				Then profile should contain UID '2'
					And profile should not contain UID '1'
				"
                .Execute();
        }

        [Test]
        public void ShouldNotReloadUidsWhenMailServerAndLoginWhereNotUpdated()
        {
            @"
				Given TargetProcess contains UID '1' for server 'pop1' and login 'login1'
					And TargetProcess contains UID '2' for server 'pop2' and login 'login2'
					And profile 'Profile1' for server 'pop1' and login 'login1' added
				When profile 'Profile1' rules where updated
				Then profile should contain UID '1'
					And profile should not contain UID '2'
					And UIDs should not be reloaded
				"
                .Execute();
        }

        [Given("TargetProcess contains UID '$messageUid' for server '$mailServer' and login '$login'")]
        public void SetUid(string messageUid, string mailServer, string login)
        {
            _uids.Add(new MessageUidDTO { MailLogin = login, MailServer = mailServer, UID = messageUid });
        }

        [When("profile '$profileName' for server '$mailServer' and login '$login' added")]
        public void AddProfile(string profileName, string mailServer, string login)
        {
            //_uids.ForEach(x => x.QueryResultCount = _uids.Count);
            ObjectFactory.GetInstance<TransportMock>()
                .On<RetrieveAllMessageUidsQuery>()
                .Reply(x =>
                {
                    _queryCount++;
                    return _uids.Where(uid => uid.MailLogin == login && uid.MailServer == mailServer)
                        .Select(uid => new MessageUidQueryResult { Dtos = new[] { uid }, QueryResultCount = _uids.Count })
                        .ToArray();
                });

            ObjectFactory.GetInstance<TransportMock>().AddProfile(profileName,
                ProfileSettings(mailServer, login));
        }

        private static ProjectEmailProfile ProfileSettings(string mailServer, string login)
        {
            return new ProjectEmailProfile
            {
                Login = login,
                MailServer = mailServer,
                Password = "pass",
                Port = 2,
                Protocol = "pop3",
                Rules = "then attach to project 1",
                UseSSL = true
            };
        }

        [When("profile '$profileName' server set to '$mailServer' and login to '$login'")]
        public void UpdatedProfile(string profileName, string mailServer, string login)
        {
            ObjectFactory.GetInstance<TransportMock>().ResetAllOnMessageHandlers();
            ObjectFactory.GetInstance<TransportMock>()
                .On<RetrieveAllMessageUidsQuery>()
                .Reply(x =>
                {
                    _queryCount++;
                    return _uids.Where(uid => uid.MailLogin == login && uid.MailServer == mailServer)
                        .Select(uid => new MessageUidQueryResult { Dtos = new[] { uid }, QueryResultCount = _uids.Count })
                        .ToArray();
                });
            var pluginProfileDto = new PluginProfileDto
            {
                Name = profileName,
                Settings = ProfileSettings(mailServer, login)
            };
            UpdateProfile(pluginProfileDto);
        }

        private static void UpdateProfile(PluginProfileDto pluginProfileDto)
        {
            var command = new ExecutePluginCommandCommand
            {
                CommandName = EmbeddedPluginCommands.AddOrUpdateProfile,
                Arguments = pluginProfileDto.Serialize()
            };

            ObjectFactory.GetInstance<TransportMock>().HandleMessageFromTp(command);
        }

        [When("profile '$profile' rules where updated")]
        public void UpdateProfile(string profileName)
        {
            var oldProfile = ObjectFactory.GetInstance<IStorageRepository>().GetProfile<ProjectEmailProfile>();
            var pluginProfileDto = new PluginProfileDto
            {
                Name = profileName,
                Settings = new ProjectEmailProfile
                {
                    Login = oldProfile.Login,
                    MailServer = oldProfile.MailServer,
                    Password = oldProfile.Password,
                    Port = 2,
                    Protocol = "pop3",
                    Rules = "then attach to project 111",
                    UseSSL = true
                }
            };

            UpdateProfile(pluginProfileDto);
        }

        [Then("profile should contain UID '$messageUid'")]
        public void ProfileShouldContainUid(string messageUid)
        {
            ObjectFactory.GetInstance<MessageUidRepository>().GetUids().Count(x => x == messageUid).Should(
                Be.EqualTo(1),
                "ObjectFactory.GetInstance<MessageUidRepository>().GetUids().Count(x => x == messageUid).Should(Be.EqualTo(1))");

            var profile = ObjectFactory.GetInstance<IStorageRepository>().GetProfile<ProjectEmailProfile>();
            var profileServerAndLogin =
                ObjectFactory.GetInstance<IStorageRepository>().Get<ProfileServerAndLogin>().FirstOrDefault();
            profileServerAndLogin.Login.Should(Be.EqualTo(profile.Login), "profileServerAndLogin.Login.Should(Be.EqualTo(profile.Login))");
            profileServerAndLogin.MailServer.Should(Be.EqualTo(profile.MailServer),
                "profileServerAndLogin.MailServer.Should(Be.EqualTo(profile.MailServer))");
        }

        [Then("profile should not contain UID '$messageUid'")]
        public void ProfileShouldNotContainUid(string messageUid)
        {
            ObjectFactory.GetInstance<MessageUidRepository>()
                .GetUids()
                .Where(x => x == messageUid)
                .Should(Be.Empty, "ObjectFactory.GetInstance<MessageUidRepository>().GetUids().Where(x => x == messageUid).Should(Be.Empty)");
        }

        [Then("UIDs should not be reloaded")]
        public void ShouldNotReloadUIDs()
        {
            _queryCount.Should(Be.EqualTo(1), "UIDs where reloaded, query to TargetProcess was sent twice");
        }
    }
}
