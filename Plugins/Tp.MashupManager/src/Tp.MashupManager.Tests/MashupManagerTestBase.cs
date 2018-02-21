using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Testing.Common;
using Tp.MashupManager.MashupStorage;
using Tp.Plugin.Core;
using Tp.Testing.Common.NUnit;

namespace Tp.MashupManager.Tests
{
    [ActionSteps]
    public class MashupManagerTestBase
    {
        protected PluginCommandResponseMessage _response;
        protected const string DefaultProfileName = MashupInfoRepository.ProfileName;
        protected const string DefaultPlaceholders = "Default";
        protected const string DefaultScript = "alert(123)";
        protected const string DefaultConfigScript = "config";

        [SetUp]
        public virtual void SetUp()
        {
            ObjectFactory.Configure(
                x =>
                {
                    x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(MashupManagerProfile).Assembly,
                        new List<Assembly>
                        {
                            typeof(ExceptionThrownLocalMessage).
                                Assembly,
                            typeof(MashupManagerProfile).Assembly
                        },
                        new Assembly[] { }));
                    x.For<IActivityLogger>().HybridHttpOrThreadLocalScoped().Use<ActivityLoggerMock>();
                    x.Forward<IActivityLogger, ActivityLoggerMock>();

                    var mashupFolderMock = MockRepository.GenerateStub<IMashupLocalFolder>();
                    mashupFolderMock.Expect(y => y.Path).Return(MashupStorageDirectory);
                    x.For<IMashupLocalFolder>().Use(mashupFolderMock);
                });
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (Directory.Exists(MashupStorageDirectory))
            {
                Directory.Delete(MashupStorageDirectory, true);
            }
        }

        public static string MashupStorageDirectory => Path.Combine(Directory.GetCurrentDirectory(), "MashupStorage");

        protected static IPluginContext PluginContext => ObjectFactory.GetInstance<IPluginContext>();

        protected static IProfile Profile => ObjectFactory.GetInstance<ISingleProfile>().Profile;

        protected static MashupManagerProfile MashupManagerProfile => Profile.GetProfile<MashupManagerProfile>();

        protected TransportMock TransportMock => ObjectFactory.GetInstance<TransportMock>();

        protected static ActivityLoggerMock Logger => ObjectFactory.GetInstance<ActivityLoggerMock>();

        protected IEnumerable<PluginMashupMessage> PluginMashupsMessages => TransportMock.TpQueue.GetMessages<PluginMashupMessage>();

        protected static IMashupScriptStorage ScriptStorage => ObjectFactory.GetInstance<IMashupScriptStorage>();

        protected static IStorageRepository StorageRepository => ObjectFactory.GetInstance<IStorageRepository>();

        [Given("no profiles created")]
        public void NoProfileCreated()
        {
        }

        [Given("profile created")]
        public void CreateProfile()
        {
            CreateProfile(DefaultProfileName);
        }

        [Given("profile '$profileName' created")]
        public void CreateProfile(string profileName)
        {
            TransportMock.AddProfile(profileName, new MashupManagerProfile());
        }

        [Given("profile created for account '$accountName'")]
        public void CreateProfileForAccount(string accountName)
        {
            CreateProfileForAccount(DefaultProfileName, accountName);
        }

        [Given("profile '$profileName' created for account '$accountName'")]
        public void CreateProfileForAccount(string profileName, string accountName)
        {
            TransportMock.AddProfile(profileName, accountName, new MashupManagerProfile());
        }

        [Given(@"profile mashups are: (?<mashupNames>([^,]+,?\s*)+)")]
        public void SetMashupNamesToProfile(string[] mashupNames)
        {
            foreach (var mashupName in mashupNames)
            {
                MashupManagerProfile.AddMashup(mashupName);

                var mashupFiles = new List<MashupFile>
                {
                    new MashupFile { FileName = mashupName + ".js", Content = DefaultScript }
                };
                ScriptStorage.SaveMashup(new Mashup(mashupFiles)
                {
                    Name = mashupName,
                    Placeholders = DefaultPlaceholders
                });
            }
        }

        [Given("mashup '$mashupName' with config file created in profile")]
        public void SetMashupNamesToProfile(string mashupName)
        {
            MashupManagerProfile.AddMashup(mashupName);
            var mashupFiles = new List<MashupFile>
            {
                new MashupFile { FileName = mashupName + ".js", Content = DefaultScript },
                new MashupFile { FileName = mashupName + ".config.js", Content = DefaultConfigScript }
            };
            ScriptStorage.SaveMashup(new Mashup(mashupFiles)
            {
                Name = mashupName,
                Placeholders = DefaultPlaceholders
            });
        }

        [Given(@"profile '$profileName' mashups are: (?<mashupNames>([^,]+,?\s*)+)")]
        public void SetMashupNamesToProfile(string profileName, string[] mashupNames)
        {
            var profile = ObjectFactory.GetInstance<IProfileCollection>().Single(p => p.Name == profileName);
            profile.GetProfile<MashupManagerProfile>().AddMashups(mashupNames);
        }

        [Then(@"profile should have following mashups: (?<mashupNames>([^,]+,?\s*)+)")]
        public void CheckProfileMashups(string[] mashupNames)
        {
            MashupManagerProfile.MashupNames.Should(Be.EquivalentTo(mashupNames),
                "MashupManagerProfile should contain mashups");

            foreach (var mashupName in mashupNames)
            {
                var mashup = GetMashupFromStorage(mashupName);
                mashup.Placeholders.Should(Be.Not.Null.And.Not.Empty, "mashup.Placeholders.Should(Be.Not.Null.And.Not.Empty)");
                mashup.Files.Should(Be.Not.Null.And.Not.Empty, "mashup.Files.Should(Be.Not.Null.And.Not.Empty)");
            }
        }

        [Then("$count mashup should be sent to TP")]
        [Then("$count mashups should be sent to TP")]
        public void CheckCreatedMashupsCount(int count)
        {
            PluginMashupsMessages.Count().Should(Be.EqualTo(count), $"{count} mashups should be sent to TP");
        }

        [Then("mashup '$mashupName' with placeholders '$placeholders' and script '$script' should be sent to TP")]
        public void CheckCreatedInTpMashup(string mashupName, string placeholders, string script)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupName.Should(Be.EqualTo(mashupName), "Mashup name should be set as expected");
            mashup.PluginMashupScripts.Single()
                .ScriptContent.Should(Be.EqualTo(script), "mashup.PluginMashupScripts.Single().ScriptContent.Should(Be.EqualTo(script))");
            CheckMashupPlaceholders(mashup, placeholders);
        }

        [Then(@"mashup '$mashupName' with placeholders '$placeholders' and fileNames '(?<fileNames>([^,]+,?\s*)+)' should be sent to TP")]
        public void CheckCreatedInTpMashupWithConfig(string mashupName, string placeholders, string[] fileNames)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupName.Should(Be.EqualTo(mashupName), "Mashup name should be set as expected");
            mashup.PluginMashupScripts.Select(x => x.FileName)
                .ToArray()
                .Should(Be.EquivalentTo(fileNames),
                    "mashup.PluginMashupScripts.Select(x => x.FileName).ToArray().Should(Be.EquivalentTo(fileNames))");
            CheckMashupPlaceholders(mashup, placeholders);
        }

        [Then("'$fileName' file of '$mashupName' mashup sended to TP should contain '$script' script")]
        public void CheckSendedToTpMashupFileContent(string fileName, string mashupName, string script)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupName.Should(Be.EqualTo(mashupName), "Mashup name should be set as expected");
            mashup.PluginMashupScripts.First(x => x.FileName.EqualsIgnoreCase(fileName))
                .ScriptContent.Should(Be.EqualTo(script), "Mashup has expected script file with expected content");
        }

        [Then("mashup '$mashupName' with accounts '$accounts' and placeholders '$placeholders' and script '$script' should be sent to TP")]
        public void CheckCreatedInTpMashup(string mashupName, string accounts, string placeholders, string script)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupName.Should(Be.EqualTo(mashupName), "Mashup name should be set as expected");
            mashup.PluginMashupScripts.Select(s => s.ScriptContent)
                .ToArray()
                .Should(Be.EquivalentTo(new[] { script, MashupConfig.AccountsConfigLine(accounts) }),
                    "Mashup scripts should have expected content");
            CheckMashupPlaceholders(mashup, placeholders);

            var scriptNames = mashup.PluginMashupScripts.Select(s => s.FileName).ToArray();
            scriptNames.Count().Should(Be.EqualTo(2), "Mashup should have two files");
            scriptNames.Should(Contains.Item(Mashup.AccountCfgFileName), "Mashup should have config file");
            scriptNames.Count(n => n.EndsWith("js")).Should(Be.EqualTo(1), "Mashup should have js file");
        }

        [Then("mashup '$mashupName' should be enabled")]
        public void CheckCreatedInTpMashupIsEnabled(string mashupName)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupMetaInfo.IsEnabled.Should(Be.True, "Mashup should be enabled");
        }

        [Then("mashup '$mashupName' should be disabled")]
        public void CheckCreatedInTpMashupIsDisabled(string mashupName)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupMetaInfo.IsEnabled.Should(Be.False, "Mashup should be disabled");
        }

        [Then("mashup '$mashupName' should be marked as installed from library")]
        public void CheckCreatedInTpMashupIsInstalledFromLibrary(string mashupName)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupMetaInfo.PackageName.IsNullOrEmpty()
                .Should(Be.False, "Mashup should be marked as installed from library");
        }

        [Then("mashup '$mashupName' should be marked as not installed from library")]
        public void CheckCreatedInTpMashupIsNotInstalledFromLibrary(string mashupName)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupMetaInfo.PackageName.IsNullOrEmpty()
                .Should(Be.True, "Mashup should be marked as not installed from library");
        }

        [Then("mashup '$mashupName' should be marked as last modified by '$userName' at '$timestamp'")]
        public void CheckLastModificationInfo(string mashupName, string userName, ulong timestamp)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.MashupMetaInfo.LastModifiedBy.Should(Be.Not.Null, "LastModifiedBy should be set");
            mashup.MashupMetaInfo.LastModifiedBy.Name.Should(Be.EqualTo(userName), $"Mashup should be marked as last modified by {userName}");
            mashup.MashupMetaInfo.LastModificationDate.Should(Be.EqualTo(timestamp), "Mashup should have last modification time set as expected");
        }

        private static void CheckMashupPlaceholders(PluginMashupMessage mashup, string expectedPlaceholders)
        {
            mashup.Placeholders.Select(p => p.ToLower())
                .Should(Be.EquivalentTo(expectedPlaceholders.Split(',').Select(p => p.Trim().ToLower())),
                    "Mashup placeholders should be set as expected");
        }

        [Then("default mashup '$mashupName' with accounts '$accountName' should be sent to TP")]
        public void CheckCreatedInTpMashup(string mashupName, string accountName)
        {
            CheckCreatedInTpMashup(mashupName, accountName, DefaultPlaceholders, DefaultScript);
        }

        [Then("mashup '$mashupName' with plugin name '$pluginName' should be sent to TP")]
        public void CheckMashupPluginNameInTp(string mashupName, string pluginName)
        {
            var mashup = GetMashupMessageByName(mashupName);
            mashup.PluginName.Value.Should(Be.EqualTo(pluginName), "mashup.PluginName.Value.Should(Be.EqualTo(pluginName))");
        }

        [Then("default mashup '$mashupName' should be sent to TP")]
        public void CheckDefaultMashupSentToTp(string mashupName)
        {
            CheckCreatedInTpMashup(mashupName, DefaultPlaceholders, DefaultScript);
        }

        [Then("$count mashup should be in profile storage")]
        public void CheckMashupStorage(int count)
        {
            Directory.GetDirectories(MashupStorageDirectory)
                .Count()
                .Should(Be.EqualTo(count), "Directory.GetDirectories(MashupStorageDirectory).Count().Should(Be.EqualTo(count))");
        }

        [Then(
            @"profile storage should contain mashup '$mashupName' with placeholders '$placeholders' and files '(?<fileNames>([^,]+,?\s*)+)'"
            )]
        public void CheckMashupFilesInStorage(string mashupName, string placeholders, string[] fileNames)
        {
            var mashup = ScriptStorage.GetMashup(mashupName);

            mashup.Name.Should(Be.EqualTo(mashupName), "mashup.Name.Should(Be.EqualTo(mashupName))");
            mashup.Placeholders.ToLower()
                .Should(Be.EqualTo(placeholders.ToLower()), "mashup.Placeholders.ToLower().Should(Be.EqualTo(placeholders.ToLower()))");
            mashup.Files.Select(x => x.FileName)
                .ToArray()
                .Should(Be.EquivalentTo(fileNames), "mashup.Files.Select(x => x.FileName).ToArray().Should(Be.EquivalentTo(fileNames))");

            MashupManagerProfile.ContainsMashupName(mashupName).Should(Be.True,
                "MashupManagerProfile should contain mashup");
        }

        [Then("'$fileName' file of '$mashupName' mashup in storage should contain '$script' script")]
        public void CheckMashupFileContentInStorage(string fileName, string mashupName, string script)
        {
            var mashup = ScriptStorage.GetMashup(mashupName);

            mashup.Name.Should(Be.EqualTo(mashupName), "mashup.Name.Should(Be.EqualTo(mashupName))");
            mashup.Files.First(x => x.FileName.EqualsIgnoreCase(fileName))
                .Content.Should(Be.EqualTo(script),
                    "mashup.Files.First(x => x.FileName.EqualsIgnoreCase(fileName)).Content.Should(Be.EqualTo(script))");
        }

        [Then("mashup '$mashupName' with placeholders '$placeholders' and script '$script' should be in profile storage")]
        public void CheckMashupInStorage(string mashupName, string placeholders, string script)
        {
            var mashup = ScriptStorage.GetMashup(mashupName);

            mashup.Name.Should(Be.EqualTo(mashupName), "mashup.Name.Should(Be.EqualTo(mashupName))");
            mashup.Placeholders.ToLower()
                .Should(Be.EqualTo(placeholders.ToLower()), "mashup.Placeholders.ToLower().Should(Be.EqualTo(placeholders.ToLower()))");
            mashup.Files.First().Content.Should(Be.EqualTo(script), "mashup.Files.First().Content.Should(Be.EqualTo(script))");

            MashupManagerProfile.ContainsMashupName(mashupName).Should(Be.True,
                "MashupManagerProfile should contain mashup");
        }

        [Then("command should return validation error for '$fieldName' field '$errorMessage'")]
        public void CheckValidateError(string fieldName, string errorMessage)
        {
            _response.PluginCommandStatus.Should(Be.EqualTo(PluginCommandStatus.Fail),
                "_response.PluginCommandStatus.Should(Be.EqualTo(PluginCommandStatus.Fail))");
            var error = _response.ResponseData.Deserialize<PluginProfileErrorCollection>()
                .SingleOrDefault(e => e.FieldName == fieldName);
            error.Should(Be.Not.Null, "error.Should(Be.Not.Null)");
            error.Message.Should(Be.EqualTo(errorMessage), "error.Message.Should(Be.EqualTo(errorMessage))");
        }

        [Then("default mashup '$mashupName' should be in profile storage")]
        public void CheckDefaultMashupInStorage(string mashupName)
        {
            CheckMashupInStorage(mashupName, DefaultPlaceholders, DefaultScript);
        }

        [Then("mashup '$mashupName' should be cleared in TP and profile")]
        public void MashupShouldBeCleared(string mashupName)
        {
            MashupShouldBeClearedInProfile(mashupName);
            MashupShouldBeClearedInTp(mashupName);
        }

        [Then("mashup '$mashupName' with plugin name '$pluginName' should be cleared in TP")]
        public void MashupShouldBeCleared(string mashupName, string pluginName)
        {
            var mashupCommand = GetMashupMessageByName(mashupName);
            mashupCommand.PluginName.Value.Should(Be.EqualTo(pluginName), "mashupCommand.PluginName.Value.Should(Be.EqualTo(pluginName))");
            mashupCommand.Placeholders.Should(Be.Empty, "mashupCommand.Placeholders.Should(Be.Empty)");
            mashupCommand.PluginMashupScripts.Should(Be.Empty, "mashupCommand.PluginMashupScripts.Should(Be.Empty)");
        }

        [Then("mashup '$mashupName' should be cleared in profile")]
        public void MashupShouldBeClearedInProfile(string mashupName)
        {
            MashupManagerProfile.ContainsMashupName(mashupName)
                .Should(Be.False, "MashupManagerProfile should not contain mashup");
            GetMashupFromStorage(mashupName).Should(Be.Null, "Mashup Storage should not contain mashup");
        }

        [Then("mashup '$mashupName' should be cleared in TP")]
        public void MashupShouldBeClearedInTp(string mashupName)
        {
            var mashupCommand = GetMashupMessageByName(mashupName);
            mashupCommand.Placeholders.Should(Be.Empty, "mashupCommand.Placeholders.Should(Be.Empty)");
            mashupCommand.PluginMashupScripts.Should(Be.Empty, "mashupCommand.PluginMashupScripts.Should(Be.Empty)");
        }

        protected Mashup GetMashupFromStorage(string mashupName)
        {
            return ScriptStorage.GetMashup(mashupName);
        }

        protected PluginMashupMessage GetMashupMessageByName(string mashupName)
        {
            return PluginMashupsMessages.SingleOrDefault(m => m.MashupName == mashupName);
        }
    }
}
