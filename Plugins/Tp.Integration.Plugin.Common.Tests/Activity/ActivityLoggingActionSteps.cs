// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus;
using NServiceBus.Unicast.Transport;
using StructureMap;
using Tp.Core;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NUnit;
using log4net.Core;
using log4net.Repository.Hierarchy;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.Integration.Plugin.Common.Tests.Activity
{
    [ActionSteps]
    public class ActivityLoggingActionSteps
    {
        private const string ACTIVITY_LOGGER_NAME = ActivityLoggerRegistry.ActivityLoggerName;
        private const string ERROR_LOGGER_NAME = ActivityLoggerRegistry.ErrorLoggerName;

        protected ActivityLoggingContext Context => ObjectFactory.GetInstance<ActivityLoggingContext>();

        [Given("profile '$profileName' for empty account created")]
        public void CreateProfileWithoutAccount(string profileName)
        {
            CreateProfileForPlugin(profileName, AccountName.Empty.Value);
        }

        [Given("profile '$profileName' for account '$accountName' created")]
        public void CreateProfileForPlugin(string profileName, string accountName)
        {
            Context.TransportMock.AddProfile(profileName, accountName);
        }

        [When(@"plugin receives '$commandName' command for account '$accountName' and profile '$profileName'")]
        public void ExecuteCommand(string commandName, string account, string profileName)
        {
            var profile = new PluginProfileDto { Name = profileName, Settings = new SampleProfileSerialized() };
            var cmd = new ExecutePluginCommandCommand { CommandName = commandName, Arguments = profile.Serialize() };
            Context.TransportMock.HandleMessageFromTp(
                new List<HeaderInfo>
                {
                    new HeaderInfo { Key = BusExtensions.ACCOUNTNAME_KEY, Value = account },
                    new HeaderInfo { Key = BusExtensions.PROFILENAME_KEY, Value = profileName }
                },
                cmd);
        }

        [When("retrieving activity logger")]
        public void RetrieveActivityLogger()
        {
            Context.Loggers.Add(ObjectFactory.GetInstance<ILogManager>().GetLogger(ACTIVITY_LOGGER_NAME));
        }

        [When("retrieving plugin activity logger for account '$accountName' and profile '$profileName'")]
        public void RetrieveActivityLoggerForAccountAndProfile(string accountName, string profileName)
        {
            ObjectFactory.GetInstance<IBus>().SetIn(new AccountName(accountName));
            ObjectFactory.GetInstance<IBus>().SetIn(new ProfileName(profileName));
            RetrieveActivityLogger();
        }

        [When("retrieving error logger")]
        public void RetrieveErrorLogger()
        {
            Context.Loggers.Add(ObjectFactory.GetInstance<ILogManager>().GetLogger(ERROR_LOGGER_NAME));
        }

        [When("retrieving activity logs by end date '$date'")]
        public void RetrieveActivityLogsByEndDate(string date)
        {
            var dateTime = DateTime.ParseExact(date, "yyyy-MM-dd hh:mm:ss,fff tt", CultureInfo.CurrentCulture);
            Context.Activities.Add(
                ObjectFactory.GetInstance<IProfile>().Log.GetBy(new ActivityFilter { DateRange = new DateRange(null, dateTime) }));
        }

        [When("retrieving activity logs by start date '$date'")]
        public void RetrieveActivityLogsByStartDate(string date)
        {
            var dateTime = DateTime.ParseExact(date, "yyyy-MM-dd hh:mm:ss,fff tt", CultureInfo.CurrentCulture);
            Context.Activities.Add(
                ObjectFactory.GetInstance<IProfile>().Log.GetBy(new ActivityFilter { DateRange = new DateRange(dateTime, null) }));
        }

        [When("retrieving activity logs")]
        public void RetrieveActivityLogs()
        {
            Context.Activities.Add(
                ObjectFactory.GetInstance<IProfile>().Log.GetBy(new ActivityFilter { DateRange = DateRange.Empty }));
        }

        [When("retrieving error logs")]
        public void RetrieveErrorLogs()
        {
            Context.Activities.Add(
                ObjectFactory.GetInstance<IProfile>().Log.GetBy(new ActivityFilter
                    { Type = ActivityType.Errors, DateRange = DateRange.Empty }));
        }

        [When("removing activity logger for the profile")]
        public void RemoveActivityLogger()
        {
            ObjectFactory.GetInstance<IProfile>().Log.Remove();
        }

        [When("clearing activity log for the profile")]
        public void ClearActivityLog()
        {
            ObjectFactory.GetInstance<IProfile>().Log.ClearBy(new ActivityFilter { Type = ActivityType.All });
        }

        [When("unhandled exception is thrown during message handling for account '$accountName' and profile '$profileName'")]
        public void UnhandledExceptionIsThrownDuringMessageHandling(string accountName, string profileName)
        {
            try
            {
                Context.TransportMock.HandleLocalMessage(new List<HeaderInfo>
                    {
                        new HeaderInfo { Key = BusExtensions.ACCOUNTNAME_KEY, Value = accountName },
                        new HeaderInfo { Key = BusExtensions.PROFILENAME_KEY, Value = profileName }
                    },
                    new SampleSagaMessage());
            }
            catch
            {
                //do nothing
            }
        }

        [Then("activity logger for account '$accountName' and profile '$profileName' should be retrieved")]
        public void ActivityLoggerForAccountAndProfileShouldBeRetrieved(string accountName, string profileName)
        {
            RetrieveLogger(ACTIVITY_LOGGER_NAME, accountName, profileName);
        }

        [Then("error logger for account '$accountName' and profile '$profileName' should be retrieved")]
        public void RetrieveErrorLoggerForAccountAndProfile(string accountName, string profileName)
        {
            RetrieveLogger(ERROR_LOGGER_NAME, accountName, profileName);
        }

        [Then("activity object with $numberOfRecords records should be retrieved")]
        public void ActivityShouldBeRetrieved(int numberOfRecords)
        {
            var activity = Context.Activities.FirstOrDefault();
            activity.Should(Be.Not.Null, "activity.Should(Be.Not.Null)");
            activity.Records.Count().Should(Be.EqualTo(numberOfRecords), "activity.Records.Count().Should(Be.EqualTo(numberOfRecords))");
        }

        [Then("no records should be retrieved")]
        public void NoRecordsShouldBeRetrieved()
        {
            var activity = Context.Activities.FirstOrDefault();
            activity.Should(Be.Not.Null, "activity.Should(Be.Not.Null)");
            activity.Records.Should(Be.Empty, "activity.Records.Should(Be.Empty)");
        }

        [Then("logs for account '$accountName' and profile '$profileName' should be removed")]
        public void PluginShouldBeRemoved(string accountName, string profileName)
        {
            var mock = ObjectFactory.GetInstance<Log4NetFileRepositoryMock>();

            mock.RemovedFolders.Count.Should(Be.EqualTo(1), "mock.RemovedFolders.Count.Should(Be.EqualTo(1))");
            mock.RemovedFolders.Select(x => x.Contains(accountName) && x.Contains(profileName))
                .Count()
                .Should(Be.EqualTo(1),
                    "mock.RemovedFolders.Select(x => x.Contains(accountName) && x.Contains(profileName)).Count().Should(Be.EqualTo(1))");
        }

        [Then("activity log for account '$accountName' and profile '$profileName' should be removed")]
        public void ActivityLogShouldBeRemoved(string accountName, string profileName)
        {
            var mock = ObjectFactory.GetInstance<Log4NetFileRepositoryMock>();

            mock.RemovedFiles.Count.Should(Be.EqualTo(3), "mock.RemovedFiles.Count.Should(Be.EqualTo(3))");
            mock.RemovedFiles.All(x => x.GetFileNameWithoutExtension().StartsWith("Activity"))
                .Should(Be.True, "mock.RemovedFiles.All(x => x.GetFileNameWithoutExtension().StartsWith(\"Activity\")).Should(Be.True)");
            mock.RemovedFiles.Select(x => x.Contains(accountName) && x.Contains(profileName))
                .Count()
                .Should(Be.EqualTo(3),
                    "mock.RemovedFiles.Select(x => x.Contains(accountName) && x.Contains(profileName)).Count().Should(Be.EqualTo(3))");
        }

        [Then("error log for account '$accountName' and profile '$profileName' should be removed")]
        public void ErrorLogShouldBeRemoved(string accountName, string profileName)
        {
            var mock = ObjectFactory.GetInstance<Log4NetFileRepositoryMock>();

            mock.RemovedFiles.Count.Should(Be.EqualTo(3), "mock.RemovedFiles.Count.Should(Be.EqualTo(3))");
            mock.RemovedFiles.All(x => x.GetFileNameWithoutExtension().StartsWith("Error"))
                .Should(Be.True, "mock.RemovedFiles.All(x => x.GetFileNameWithoutExtension().StartsWith(\"Error\")).Should(Be.True)");
            mock.RemovedFiles.Select(x => x.Contains(accountName) && x.Contains(profileName))
                .Count()
                .Should(Be.EqualTo(3),
                    "mock.RemovedFiles.Select(x => x.Contains(accountName) && x.Contains(profileName)).Count().Should(Be.EqualTo(3))");
        }

        [Then("no records should be written to activity log for account '$accountName' and profile '$profileName'")]
        public void NoRecordsShouldBeWrittenToActivityLog(string accountName, string profileName)
        {
            var fullLogName = accountName + "\\" + profileName + "\\" + "PluginActivityLogger";
            var logger = ObjectFactory.GetInstance<FakeActivityLogger>();
            logger.LoggersUsed.Where(x => x == fullLogName)
                .ToList()
                .Should(Be.Empty, "logger.LoggersUsed.Where(x => x == fullLogName).ToList().Should(Be.Empty)");
        }

        [Then("activity log for profile '$profileName' for account '$accountName' should be written")]
        public void ActivityLogShouldBeWritten(string profileName, string accountName)
        {
            var fullLogName = accountName + "\\" + profileName + "\\" + "PluginActivityLogger";
            var logger = ObjectFactory.GetInstance<FakeActivityLogger>();
            logger.LoggersUsed.Where(x => x == fullLogName)
                .ToList()
                .Should(Be.Not.Empty, "logger.LoggersUsed.Where(x => x == fullLogName).ToList().Should(Be.Not.Empty)");
        }

        [Then("error log for profile '$profileName' for account '$accountName' should be written")]
        public void ErrorLogShouldBeWritten(string profileName, string accountName)
        {
            var fullLogName = accountName + "\\" + profileName + "\\" + "PluginErrorLogger";
            var logger = ObjectFactory.GetInstance<FakeActivityLogger>();
            logger.LoggersUsed.Where(x => x == fullLogName)
                .ToList()
                .Should(Be.Not.Empty, "logger.LoggersUsed.Where(x => x == fullLogName).ToList().Should(Be.Not.Empty)");
        }

        private void RetrieveLogger(string loggerName, string accountName, string profileName)
        {
            var logger = Context.Loggers
                .Select(x => x.Logger)
                .OfType<Logger>()
                .FirstOrDefault(x => x.Name.Contains(loggerName) && x.Name.Contains(accountName) && x.Name.Contains(profileName));

            logger.Should(Be.Not.Null, "logger.Should(Be.Not.Null)");
            CheckAppenders(logger, accountName, profileName);
        }

        private static void CheckAppenders(IAppenderAttachable logger, string accountName, string profileName)
        {
            logger.Appenders.Should(Be.Not.Empty, "logger.Appenders.Should(Be.Not.Empty)");
            logger.Appenders.OfType<PluginRollingFileAppender>()
                .Should(Be.Not.Empty, "logger.Appenders.OfType<PluginRollingFileAppender>().Should(Be.Not.Empty)");

            var appender = logger.Appenders.OfType<PluginRollingFileAppender>().First();
            appender.File.Contains(accountName).Should(Be.True, "appender.File.Contains(accountName).Should(Be.True)");
            appender.File.Contains(profileName).Should(Be.True, "appender.File.Contains(profileName).Should(Be.True)");
        }
    }
}
