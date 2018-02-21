//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Core;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.SourceControl.Commands;
using Tp.SourceControl.Settings;
using Tp.Subversion.Context;
using Tp.Subversion.Subversion;
using Tp.Testing.Common.NUnit;

namespace Tp.Subversion.EditProfileFeature
{
    [ActionSteps]
    public class CommandActionSteps
    {
        private SubversionPluginProfile _settings;
        private PluginCommandResponseMessage _response;
        private PluginProfileErrorCollection _errors;

        [Given("unsaved plugin profile")]
        public void GivenProfile()
        {
            _settings = new SubversionPluginProfile();
        }

        [Given("profile repository path is '$uri'")]
        public void GivenRepositoryPath(string uri)
        {
            _settings.Uri = uri;
        }

        [Given("profile local repository path is '$uri'")]
        public void GivenLocalRepositoryPath(string uri)
        {
            var settings = LocalRepositorySettings.Create(string.Empty);
            _settings.Uri = uri.Replace(@".\", settings.Uri + "/");
        }

        [Given("profile login is '$login'")]
        public void GivenLogin(string login)
        {
            _settings.Login = login;
        }

        [Given("profile password is '$password'")]
        public void GivenPassword(string password)
        {
            _settings.Password = password;
        }

        [Given("profile start revision is $startRevision")]
        public void GivenStartRevision(string startRevision)
        {
            _settings.StartRevision = startRevision;
        }

        [Given("user mapping is:")]
        public void UpdateUserMapping(string subversion, string targetprocess)
        {
            var tpUserId = GetTpUserIdByUserName(targetprocess);
            _settings.UserMapping.Add(new MappingElement
            {
                Key = subversion,
                Value = new MappingLookup { Id = tpUserId, Name = targetprocess }
            });
        }

        private int GetTpUserIdByUserName(string targetprocess)
        {
            var tpUser = Context.TpUsers.Find(x => x.Name == targetprocess);
            return tpUser != null ? tpUser.Id : EntityId.Next();
        }

        [When("checked connection")]
        public void WhenCheckedConnection()
        {
            HandlePluginCommand("CheckConnection", new PluginProfileDto { Name = "Test Profile", Settings = _settings }.Serialize());

            _response = Context.Transport.TpQueue.GetMessages<PluginCommandResponseMessage>().Single();
            _errors = _response.ResponseData.Deserialize<PluginProfileErrorCollection>();
        }

        [When("automapping requested")]
        public void RequestAutomapping()
        {
            var settings = new ConnectionSettings { UserMapping = _settings.UserMapping };
            var args = new AutomapVcsToTpUsersCommandArgs { Connection = settings, TpUsers = Context.TpUsers.ToList() };

            HandlePluginCommand(ObjectFactory.GetInstance<AutomapVcsToTpUsersCommand>().Name, args.Serialize());

            _response = Context.Transport.TpQueue.GetMessages<PluginCommandResponseMessage>().Single();
        }

        [Then("mapping result info should be: $mappingResult")]
        public void MappingResultShouldBe(string mappingResult)
        {
            _response.PluginCommandStatus.Should(Be.EqualTo(PluginCommandStatus.Succeed),
                "_response.PluginCommandStatus.Should(Be.EqualTo(PluginCommandStatus.Succeed))");
            var message = _response.ResponseData.Deserialize<AutomapVcsToTpUsersCommandResponse>();
            message.Comment.Should(Be.EqualTo(mappingResult), "message.Comment.Should(Be.EqualTo(mappingResult))");
        }

        [Then("users should be mapped this way:")]
        public void AssertUserMapping(string svnuser, string tpuser)
        {
            var userMapping = _response.ResponseData.Deserialize<AutomapVcsToTpUsersCommandResponse>().UserLookups;
            userMapping[svnuser].Name.Should(Be.EqualTo(tpuser), "userMapping[svnuser].Name.Should(Be.EqualTo(tpuser))");
        }

        [Given("current date is $currentDate")]
        public void SetCurrentDate(DateTime currentDate)
        {
            CurrentDate.Setup(() => currentDate);
        }

        [When("saved")]
        public void WhenSaved()
        {
            HandlePluginCommand("AddProfile", new PluginProfileDto { Name = "Test Profile", Settings = _settings }.Serialize());

            _response = Context.Transport.TpQueue.GetMessages<PluginCommandResponseMessage>().Single();
            _errors = new PluginProfileErrorCollection();

            if (_response.PluginCommandStatus == PluginCommandStatus.Fail)
            {
                _errors = _response.ResponseData.Deserialize<PluginProfileErrorCollection>();
            }
        }

        private void HandlePluginCommand(string commandName, string args)
        {
            var command = new ExecutePluginCommandCommand { CommandName = commandName, Arguments = args };

            Context.Transport.TpQueue.Clear();
            Context.Transport.HandleMessageFromTp(command);
        }

        [Then("no errors should occur for $fieldName")]
        public void NoErrorsShouldOccurFor(string fieldName)
        {
            // TODO: remove select when updated sdk with beautiful ToString version
            var errors = _errors.Where(x => x.FieldName == fieldName).Select(x => x.Message).ToList();
            errors.Should(Be.Empty, "errors.Should(Be.Empty)");
        }

        [Then("no errors should occur")]
        public void NoErrorsShouldOccur()
        {
            // TODO: remove select when updated sdk with beautiful ToString version
            var errors = _errors.Select(x => x.Message).ToList();
            errors.Should(Be.Empty, "errors.Should(Be.Empty)");
        }

        [Then(@"error should occur for Uri: ""$errorMessage""")]
        public void ErrorsShouldOccur(string errorMessage)
        {
            IEnumerable<string> messages = _errors.Where(x => x.FieldName == "Uri").Select(x => x.Message).ToList();
            var errorMessages = errorMessage.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var message in messages)
            {
                errorMessages.Should(Contains.Item(message), "errorMessages.Should(Contains.Item(message))");
            }
        }

        [Then(@"error should occur for $fieldName: ""$errorMessage""")]
        public void ErrorShouldOccur(string fieldName, string errorMessage)
        {
            IEnumerable<string> messages = _errors.Where(x => x.FieldName == fieldName).Select(x => x.Message).ToList();
            messages.Should(Contains.Item(errorMessage), "messages.Should(Contains.Item(errorMessage))");
        }

        [Then("error should occur for $fieldName")]
        public void ErrorShouldOccur(string fieldName)
        {
            ErrorShouldOccur(fieldName, string.Empty);
        }

        [Then("$mappedUsersAmount users should be mapped at whole")]
        public void UsersShouldBeMapped(int mappedUsersAmount)
        {
            var userMapping = _response.ResponseData.Deserialize<AutomapVcsToTpUsersCommandResponse>().UserLookups;
            userMapping.Count.Should(Be.EqualTo(mappedUsersAmount), "userMapping.Count.Should(Be.EqualTo(mappedUsersAmount))");
        }


        public VcsPluginContext Context
        {
            get { return ObjectFactory.GetInstance<VcsPluginContext>(); }
        }
    }
}
