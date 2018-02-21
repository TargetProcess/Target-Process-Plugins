// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Validation
{
    [ActionSteps]
    [TestFixture]
    [Category("PartPlugins1")]
    public class ValidateProfileSpecs
    {
        private BugzillaProfile _settings;
        private PluginCommandResponseMessage _response;
        private PluginProfileErrorCollection _errors;

        [SetUp]
        public void Init()
        {
            ObjectFactory.Configure(x =>
            {
                x.For<TransportMock>().Use(
                    TransportMock.CreateWithoutStructureMapClear(typeof(BugzillaProfile).Assembly,
                        typeof(BugzillaProfile).Assembly));
                x.For<IAssembliesHost>().Singleton().Use(new PredefinedAssembliesHost(new[] { typeof(BugzillaProfile).Assembly }));
                x.AddRegistry<PluginRegistry>();
            });
        }

        [Test]
        public void OnlyUniqueBugzillaUsersShouldBeAllowed()
        {
            @"Given unsaved plugin profile
			And user mapping is:
			|bugzilla|targetprocess|
			|bugzillauser1|tpuser1|
			|bugzillauser1|tpuser2|
				When saved
				Then error should occur for UserMapping: ""Can't map a Bugzilla user to TargetProcess user twice.""
				"
                .Execute();
        }

        [Given("unsaved plugin profile")]
        public void GivenProfile()
        {
            _settings = BugzillaContext.GetBugzillaProfile(1);
        }

        [Given("user mapping is:")]
        public void UpdateUserMapping(string bugzilla, string targetprocess)
        {
            _settings.UserMapping.Add(new MappingElement
                { Key = bugzilla, Value = new MappingLookup { Id = EntityId.Next(), Name = targetprocess } });
        }

        [When("saved")]
        public void WhenSaved()
        {
            HandlePluginCommand("AddProfile", new PluginProfileDto { Name = "Test Profile", Settings = _settings }.Serialize());

            _response = TransportMock.TpQueue.GetMessages<PluginCommandResponseMessage>().Single();
            _errors = new PluginProfileErrorCollection();

            if (_response.PluginCommandStatus == PluginCommandStatus.Fail)
            {
                _errors = _response.ResponseData.Deserialize<PluginProfileErrorCollection>();
            }
        }

        private static TransportMock TransportMock
        {
            get { return ObjectFactory.GetInstance<TransportMock>(); }
        }

        [Then(@"error should occur for $fieldName: ""$errorMessage""")]
        public void ErrorShouldOccur(string fieldName, string errorMessage)
        {
            _errors.Any(x => x.FieldName == fieldName && x.Message == errorMessage)
                .Should(Be.True, "_errors.Any(x => x.FieldName == fieldName && x.Message == errorMessage).Should(Be.True)");
        }

        private void HandlePluginCommand(string commandName, string args)
        {
            var command = new ExecutePluginCommandCommand { CommandName = commandName, Arguments = args };

            TransportMock.TpQueue.Clear();
            TransportMock.HandleMessageFromTp(command);
        }
    }
}
