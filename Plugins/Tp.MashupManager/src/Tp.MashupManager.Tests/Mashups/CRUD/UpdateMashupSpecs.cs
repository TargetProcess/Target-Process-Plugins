using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.MashupManager.CustomCommands;
using Tp.Testing.Common.NBehave;

namespace Tp.MashupManager.Tests.Mashups.CRUD
{
    [ActionSteps, TestFixture]
    [Category("PartPlugins1")]
    public class UpdateMashupSpecs : MashupManagerTestBase
    {
        [Test]
        public void ShouldUpdateMashup()
        {
            @"
                Given profile created
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""mashup1"", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}]}'
                Then 1 mashup should be sent to TP
                    And mashup 'mashup1' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be sent to TP
                    And 2 mashup should be in profile storage
                    And mashup 'mashup1' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be in profile storage
                    And default mashup 'mashup2' should be in profile storage
            ".Execute();
        }

        [Test]
        public void ShouldUpdateMashupForAccount()
        {
            @"
                Given profile created for account 'Account1'
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""mashup1"", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}]}'
                Then 1 mashup should be sent to TP
                    And mashup 'mashup1' with accounts 'Account1' and placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be sent to TP
                    And 2 mashup should be in profile storage
                    And mashup 'mashup1' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be in profile storage
                    And default mashup 'mashup2' should be in profile storage
            ".Execute();
        }

        [Test]
        public void ShouldUpdateMashupWhenNameChanged()
        {
            @"
                Given profile created
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""mashup3"", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}]}'
                Then 2 mashups should be sent to TP
                    And mashup 'mashup1' should be cleared in TP and profile
                    And mashup 'mashup3' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be sent to TP
                    And 2 mashup should be in profile storage
                    And mashup 'mashup3' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be in profile storage
                    And default mashup 'mashup2' should be in profile storage
            ".Execute();
        }

        [Test]
        public void ShouldUpdateMashupWithMetaInfo()
        {
            @"
                Given profile created
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""mashup1"",""Placeholders"":""footerplaceholder"",""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}],""MashupMetaInfo"":{""IsEnabled"":true,""LastModificationDate"":1485250272726,""LastModifiedBy"":{""Id"":1,""Name"":""Administrator Administrator""}},""OldName"":""mashup1""}'
                Then 1 mashup should be sent to TP
                    And mashup 'mashup1' with placeholders 'footerplaceholder' and script 'alert(1234567)' should be sent to TP
                    And 2 mashup should be in profile storage
                    And mashup 'mashup1' with placeholders 'footerplaceholder' and script 'alert(1234567)' should be in profile storage
                    And mashup 'mashup1' should be enabled
                    And mashup 'mashup1' should be marked as last modified by 'Administrator Administrator' at '1485250272726'
                    And default mashup 'mashup2' should be in profile storage
            ".Execute();
        }

        [Test]
        public void ShouldUpdateEnabledMashup()
        {
            @"
                Given profile created
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""mashup1"", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""MashupMetaInfo"":{""IsEnabled"":""true""}, ""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}]}'
                Then 1 mashup should be sent to TP
                    And mashup 'mashup1' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be sent to TP
                    And mashup 'mashup1' should be enabled
            ".Execute();
        }

        [Test]
        public void ShouldUpdateDisabledMashup()
        {
            @"
                Given profile created
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""mashup1"", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""MashupMetaInfo"":{""IsEnabled"":""false""},  ""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}]}'
                Then 1 mashup should be sent to TP
                    And mashup 'mashup1' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be sent to TP
                    And mashup 'mashup1' should be disabled
            ".Execute();
        }

        [Test]
        public void ShouldCompleteWithUnchangedOriginFilesWhenUpdated()
        {
            @"
                Given profile created
                    And mashup 'mashup1' with config file created in profile
                When handle UpdateMashupCommand command with args '{""Name"":""mashup3"", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}]}'
                Then 2 mashups should be sent to TP
                    And mashup 'mashup1' should be cleared in TP and profile
                    And mashup 'mashup3' with placeholders 'Default,NewPlaceholder' and fileNames 'mashup1.js,mashup1.config.js' should be sent to TP
                    And 'mashup1.js' file of 'mashup3' mashup sended to TP should contain 'alert(1234567)' script
                    And 'mashup1.config.js' file of 'mashup3' mashup sended to TP should contain 'config' script
                    And 1 mashup should be in profile storage
                    And profile storage should contain mashup 'mashup3' with placeholders 'Default,NewPlaceholder' and files 'mashup1.js,mashup1.config.js'
                    And 'mashup1.js' file of 'mashup3' mashup in storage should contain 'alert(1234567)' script
                    And 'mashup1.config.js' file of 'mashup3' mashup in storage should contain 'config' script
            ".Execute();
        }

        [Test]
        public void ShouldUpdateMashupForAccountWhenNameChanged()
        {
            @"
                Given profile created for account 'Account1'
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""mashup3"", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}]}'
                Then 2 mashups should be sent to TP
                    And mashup 'mashup1' should be cleared in TP
                    And mashup 'mashup1' should be cleared in profile
                    And mashup 'mashup3' with accounts 'Account1' and placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be sent to TP
                    And 2 mashup should be in profile storage
                    And mashup 'mashup3' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be in profile storage
                    And default mashup 'mashup2' should be in profile storage
            ".Execute();
        }

        [Test]
        public void ShouldNotUpdateWhenUpdatedNameAlredyExists()
        {
            @"
                Given profile created
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""mashup2"", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""Files"":[{""FileName"":""script.js"",""Content"":""alert(1234567)""}]}'
                Then 0 mashups should be sent to TP
                    And command should return validation error for 'Name' field 'Mashup with the same name already exists'
                    And 2 mashup should be in profile storage
                    And default mashup 'mashup1' should be in profile storage
                    And default mashup 'mashup2' should be in profile storage
            ".Execute();
        }

        [Test]
        public void ShouldNotUpdateMashupIfNameContainsInvalidChars()
        {
            @"
                Given profile created
                    And profile mashups are: mashup1
                When handle UpdateMashupCommand command with args '{""Name"":""mashup%^&"", ""OldName"":""mashup1"", ""Placeholders"":""Default"", ""Files"":[{""FileName"":""script.js"",""Content"":""alert(1234567)""}]}'
                Then 0 mashups should be sent to TP
                    And command should return validation error for 'Name' field 'You can only use letters, numbers, space and underscore symbol in Mashup name'
            ".Execute();
        }

        [Test]
        public void ShouldTrimMashupNameBeforeUpdate()
        {
            @"
                Given profile created
                    And profile mashups are: mashup1, mashup2
                When handle UpdateMashupCommand command with args '{""Name"":""  mashup3  "", ""OldName"":""mashup1"", ""Placeholders"":""Default, NewPlaceholder"", ""Files"":[{""FileName"":""mashup1.js"",""Content"":""alert(1234567)""}]}'
                Then 2 mashups should be sent to TP
                    And mashup 'mashup1' should be cleared in TP and profile
                    And mashup 'mashup3' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be sent to TP
                    And 2 mashup should be in profile storage
                    And mashup 'mashup3' with placeholders 'Default,NewPlaceholder' and script 'alert(1234567)' should be in profile storage
                    And default mashup 'mashup2' should be in profile storage
            ".Execute();
        }

        [When("handle UpdateMashupCommand command with args '$args'")]
        public void HandleUpdateMashupCommand(string args)
        {
            var command = new UpdateMashupCommand();
            _response = command.Execute(args);
        }
    }
}
