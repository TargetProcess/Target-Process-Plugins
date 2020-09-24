// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
    public class SimpleTestRunImportResultsReaderFactory : ITestRunImportResultsReaderFactory
    {
        private readonly IActivityLogger _log;

        public SimpleTestRunImportResultsReaderFactory(IActivityLogger log)
        {
            _log = log;
        }

        public AbstractTestRunImportResultsReader GetResolver(TestRunImportSettings settings, StreamReader reader)
        {
            if (settings == null)
            {
                _log.Error("GetResolver member settings is null");
                throw new ArgumentNullException(nameof(settings));
            }

            switch (settings.FrameworkType)
            {
                case FrameworkTypes.FrameworkTypes.NUnit:
                    return new NUnitResultsXmlReader(_log, reader);
                case FrameworkTypes.FrameworkTypes.JUnit:
                    return new JUnitResultsXmlReader(_log, reader);
                case FrameworkTypes.FrameworkTypes.Selenium:
                    return new SeleniumResultsReader(_log, reader);
                case FrameworkTypes.FrameworkTypes.JenkinsHudson:
                    return new JenkinsHudsonResultsXmlReader(_log, reader);
                case FrameworkTypes.FrameworkTypes.Cucumber:
                    return new CucumberResultsXmlReader(_log, reader);
                default:
                    throw new ApplicationException($"Failed to get resolver for FrameworkType: {settings.FrameworkType}");
            }
        }
    }
}
