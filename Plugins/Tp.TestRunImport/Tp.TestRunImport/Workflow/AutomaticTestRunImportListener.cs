﻿// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security;
using System.Xml;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport.Messages;
using Tp.Integration.Plugin.TestRunImport.Streams;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.TestRunImportReaders;

namespace Tp.Integration.Plugin.TestRunImport.Workflow
{
    public class AutomaticTestRunImportListener
        : IHandleMessages<TickMessage>,
          IHandleMessages<TestCaseToTestPlanAddedMessage>,
          IHandleMessages<TestCaseFromTestPlanDeletedMessage>,
          IHandleMessages<TestCaseUpdatedMessage>,
          IHandleMessages<TestPlanUpdatedMessage>
    {
        private readonly IStorageRepository _storageRepository;
        private readonly ILocalBus _localBus;
        private readonly IStreamFactory _streamFactory;
        private readonly ITestRunImportResultsReaderFactory _resultsReaderFactory;
        private readonly IActivityLogger _log;

        public AutomaticTestRunImportListener(IStorageRepository storageRepository, ILocalBus localBus, IStreamFactory streamFactory,
            ITestRunImportResultsReaderFactory resultsReaderFactory, IActivityLogger log)
        {
            _storageRepository = storageRepository;
            _localBus = localBus;
            _streamFactory = streamFactory;
            _resultsReaderFactory = resultsReaderFactory;
            _log = log;
        }

        public void Handle(TickMessage message)
        {
            var profile = _storageRepository.GetProfile<TestRunImportPluginProfile>();
            if (profile.FrameworkType == FrameworkTypes.FrameworkTypes.Selenium && profile.PostResultsToRemoteUrl)
            {
                return;
            }

            try
            {
                _log.Info($"Started synchronizing at {DateTime.Now}");

                var lastModifyResults = _storageRepository.Get<LastModifyResult>();
                var lastModifyResult = lastModifyResults.Empty() ? new LastModifyResult() : lastModifyResults.Single();

                var jenkinsHudsonLastCompletedBuildNumber = profile.FrameworkType == FrameworkTypes.FrameworkTypes.JenkinsHudson
                    ? GetJenkinsHudsonLastCompletedBuildNumber(profile)
                    : null;

                if (profile.FrameworkType == FrameworkTypes.FrameworkTypes.JenkinsHudson &&
                (jenkinsHudsonLastCompletedBuildNumber == null
                    || string.CompareOrdinal(lastModifyResult.ETagHeader, jenkinsHudsonLastCompletedBuildNumber) == 0))
                {
                    _log.Info("No new modification of results source detected");
                    return;
                }

                var uri = profile.FrameworkType == FrameworkTypes.FrameworkTypes.JenkinsHudson
                    ? new Uri($"{profile.ResultsFilePath.TrimEnd('/', '\\')}/lastCompletedBuild/testReport/api/xml")
                    : new Uri(profile.ResultsFilePath);
                var factoryResult = _streamFactory.OpenStreamIfModified(uri, profile, lastModifyResult);

                _log.Info($"{(factoryResult == null ? "No new" : "New")} modification of results source detected");

                if (factoryResult != null)
                {
                    if (profile.FrameworkType == FrameworkTypes.FrameworkTypes.JenkinsHudson)
                    {
                        factoryResult.LastModifyResult.ETagHeader = jenkinsHudsonLastCompletedBuildNumber;
                    }

                    lastModifyResults.Clear();
                    lastModifyResults.Add(factoryResult.LastModifyResult);

                    using (factoryResult.Stream)
                    {
                        using (var reader = new StreamReader(factoryResult.Stream))
                        {
                            try
                            {
                                var result = _resultsReaderFactory.GetResolver(profile, reader).GetTestRunImportResults();
                                _log.Info(
                                    $"{(result.Count == 0 ? "No" : result.Count.ToString(CultureInfo.InvariantCulture))} items for import detected in results source");
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
                            catch (XmlException ex)
                            {
                                throw new ApplicationException("Error parsing results XML file", ex);
                            }
                            catch (Exception ex)
                            {
                                throw new ApplicationException("Error importing results XML file", ex);
                            }
                        }
                    }
                }
            }
            catch (UriFormatException ex)
            {
                _log.Error(ex.Message);
                throw new ApplicationException($"Specified path has invalid format. {ex.Message}", ex);
            }
            catch (ApplicationException ex)
            {
                _log.Error(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"Could not read file \"{profile.ResultsFilePath}\": {ex.Message}");
                throw new ApplicationException(
                    $"Could not read file \"{profile.ResultsFilePath}\": {ex.Message}", ex);
            }
        }

        public void Handle(TestCaseToTestPlanAddedMessage message)
        {
            var profile = _storageRepository.GetProfile<TestRunImportPluginProfile>();
            if (message.Dto != null && message.Dto.TestPlanID == profile.TestPlan)
            {
                if (_storageRepository.Get<TestCaseTestPlanDTO>(message.Dto.TestCaseTestPlanID.ToString()).FirstOrDefault() == null)
                {
                    _storageRepository.Get<TestCaseTestPlanDTO>(message.Dto.TestCaseTestPlanID.ToString()).Add(message.Dto);
                }
            }
        }

        public void Handle(TestCaseFromTestPlanDeletedMessage message)
        {
            var profile = _storageRepository.GetProfile<TestRunImportPluginProfile>();
            if (message.Dto != null && message.Dto.TestPlanID == profile.TestPlan)
            {
                if (_storageRepository.Get<TestCaseTestPlanDTO>(message.Dto.TestCaseTestPlanID.ToString()).FirstOrDefault() != null)
                {
                    _storageRepository.Get<TestCaseTestPlanDTO>(message.Dto.TestCaseTestPlanID.ToString()).Clear();
                }
            }
        }

        public void Handle(TestCaseUpdatedMessage message)
        {
            if (message.Dto != null && message.ChangedFields.Contains(TestCaseField.Name))
            {
                var caseTestPlanDtos = _storageRepository.Get<TestCaseTestPlanDTO>();
                var caseTestPlanDto = caseTestPlanDtos.FirstOrDefault(x => x.TestCaseID == message.Dto.TestCaseID);
                if (caseTestPlanDto != null)
                {
                    caseTestPlanDto.TestCaseName = message.Dto.Name;
                    _storageRepository.Get<TestCaseTestPlanDTO>(caseTestPlanDto.TestCaseTestPlanID.ToString()).ReplaceWith(caseTestPlanDto);
                }
            }
        }

        public void Handle(TestPlanUpdatedMessage message)
        {
            var profile = _storageRepository.GetProfile<TestRunImportPluginProfile>();
            if (message.Dto != null && message.Dto.TestPlanID == profile.TestPlan &&
                message.ChangedFields.Contains(TestPlanField.Name))
            {
                var caseTestPlanDtos = _storageRepository.Get<TestCaseTestPlanDTO>().ToArray();
                foreach (var caseTestPlanDto in caseTestPlanDtos)
                {
                    caseTestPlanDto.TestPlanName = message.Dto.Name;
                    _storageRepository.Get<TestCaseTestPlanDTO>(caseTestPlanDto.TestCaseTestPlanID.ToString()).ReplaceWith(caseTestPlanDto);
                }
            }
        }

        private string GetJenkinsHudsonLastCompletedBuildNumber(TestRunImportSettings profile)
        {
            if (profile.FrameworkType != FrameworkTypes.FrameworkTypes.JenkinsHudson) return null;

            try
            {
                var request =
                    WebRequest.Create(
                        new Uri($"{profile.ResultsFilePath.TrimEnd('/', '\\')}/lastCompletedBuild/buildNumber"));

                if (!(request is HttpWebRequest)) return null;

                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK) return null;
                    var responseStream = response.GetResponseStream();
                    if (responseStream == null) return null;
                    using (var reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd().Trim();
                    }
                }
            }
            catch (UriFormatException ex)
            {
                _log.Error($"Specified path has invalid format: {ex.Message}");
                throw new ApplicationException($"Specified path has invalid format: {ex.Message}", ex);
            }
            catch (NotSupportedException ex)
            {
                _log.Error($"Specified path has invalid format: {ex.Message}", ex);
                throw new ApplicationException($"The request scheme specified in requestUri is not registered: {ex.Message}", ex);
            }
            catch (SecurityException ex)
            {
                _log.Error(
                    $"The caller does not have permission to connect to the requested URI or a URI that the request is redirected to: {ex.Message}");
                throw new ApplicationException(
                    $"The caller does not have permission to connect to the requested URI or a URI that the request is redirected to: {ex.Message}", ex);
            }
        }
    }
}
