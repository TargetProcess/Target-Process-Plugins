// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Text;
using System.Web;
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport.Messages;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.TestRunImportReaders;

namespace Tp.Integration.Plugin.TestRunImport.Commands
{
    public class SeleniumResultsCommand : IPluginCommand
    {
        private readonly IStorageRepository _storageRepository;
        private readonly ILocalBus _localBus;
        private readonly ITestRunImportResultsReaderFactory _resultsReaderFactory;
        private readonly IActivityLogger _log;

        public SeleniumResultsCommand(IStorageRepository storageRepository, ILocalBus localBus,
            ITestRunImportResultsReaderFactory resultsReaderFactory, IActivityLogger log)
        {
            _storageRepository = storageRepository;
            _localBus = localBus;
            _resultsReaderFactory = resultsReaderFactory;
            _log = log;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            return new PluginCommandResponseMessage
            {
                ResponseData = OnExecute(HttpUtility.UrlDecode(args)),
                PluginCommandStatus = PluginCommandStatus.Succeed
            };
        }

        private string OnExecute(string args)
        {
            var profile = _storageRepository.GetProfile<TestRunImportPluginProfile>();

            _log.Info($"Started synchronizing at {DateTime.Now}");

            if (!string.IsNullOrEmpty(args) && profile.FrameworkType == FrameworkTypes.FrameworkTypes.Selenium)
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(args)))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        try
                        {
                            var result = _resultsReaderFactory.GetResolver(profile, reader).GetTestRunImportResults();
                            _log.Info($"{(result.Count == 0 ? "No" : result.Count.ToString())} items for import detected in results source");
                            if (result.Count > 0)
                            {
                                _localBus.SendLocal(new TestRunImportResultDetectedLocalMessage
                                {
                                    TestRunImportInfo =
                                        new TestRunImportInfo { TestRunImportResults = result.ToArray() }
                                });
                            }
                        }
                        catch (ApplicationException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(
                                $"Could not read file \"{profile.ResultsFilePath}\": {ex.Message}", ex);
                        }
                    }
                }
            }
            //return new PluginCommandResponseMessage { ResponseData = string.Empty, PluginCommandStatus = PluginCommandStatus.Succeed };
            return string.Empty;
        }

        public string Name => "SeleniumResults";

        //http://gmaps-api-issues.googlecode.com/svn/trunk/selenium/core/TestRunner.html?test=../tests/examples_stable_suite.html&auto=true&resultsUrl=http://localhost/tp2/api/v1/Plugins/Test%20Run%20Import/Profiles/Sel/Commands/SeleniumResults%3Ftoken=YWRtaW46OTRDRDg2Qzg1NjgzQUZDMzg3Qjg2QTVERTAxRTZEQzY=
        //file:///D:/Selenium/core/TestRunner.html?test=...&resultsUrl=http%3a%2f%2flocalhost%2fTargetProcess2%2f%2fSelenium%2fTestSuiteCallback.ashx%3fprofile%3SmokeTests
        //http://masivukeni.ccnmtl.columbia.edu/site_media/selenium/TestRunner.html?test=..%2Ftests%2FTestSuite.html&auto=on&resultsUrl=http://localhost/tp2/api/v1/Plugins/Test%20Run%20Import/Profiles/Sel/Commands/SeleniumResults%3Ftoken=YWRtaW46OTRDRDg2Qzg1NjgzQUZDMzg3Qjg2QTVERTAxRTZEQzY=

        /*
        C:\Users\vaskovskiy\selenium-server-standalone-2.3.0\core>TestRunner.hta "test=.
.%2Ffb_1%2Fcase1.html&auto=true&close=true&baseUrl=http%3A%2F%2Flocalhost%2Ftarg
etprocess&resultsUrl=http://localhost/TargetProcess/api/v1/Plugins/Test%20Run%20
Import/Profiles/AAA/Commands/SeleniumResults?token=YWRtaW46OTRDRDg2Qzg1NjgzQUZDM
zg3Qjg2QTVERTAxRTZEQzY="
        */
    }
}
