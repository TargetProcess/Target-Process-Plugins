// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport.Commands.Data;
using Tp.Integration.Plugin.TestRunImport.Streams;
using Tp.Integration.Plugin.TestRunImport.TestCaseResolvers;
using Tp.Integration.Plugin.TestRunImport.TestRunImportReaders;

namespace Tp.Integration.Plugin.TestRunImport.Mappers
{
    public class MappingChecker : IMappingChecker
    {
        private readonly IStreamFactory _streamFactory;
        private readonly ITestCaseResolverFactory _resolverFactory;
        private readonly ITestRunImportResultsReaderFactory _resultsReaderFactory;

        public MappingChecker(IStreamFactory streamFactory, ITestCaseResolverFactory resolverFactory,
            ITestRunImportResultsReaderFactory resultsReaderFactory)
        {
            _streamFactory = streamFactory;
            _resolverFactory = resolverFactory;
            _resultsReaderFactory = resultsReaderFactory;
        }

        public CheckMappingResult CheckMapping(TestRunImportPluginProfile settings,
            IEnumerable<TestCaseTestPlanDTO> testCaseTestPlans,
            PluginProfileErrorCollection errors)
        {
            try
            {
                var uri = settings.FrameworkType == FrameworkTypes.FrameworkTypes.JenkinsHudson
                    ? new Uri($"{settings.ResultsFilePath.TrimEnd(new[] { '/', '\\' })}/lastCompletedBuild/testReport/api/xml")
                    : new Uri(settings.ResultsFilePath);
                var factoryResult = _streamFactory.OpenStream(uri, settings);

                if (factoryResult != null)
                {
                    using (factoryResult.Stream)
                    {
                        using (var reader = new StreamReader(factoryResult.Stream))
                        {
                            try
                            {
                                var result = _resultsReaderFactory.GetResolver(settings, reader).GetTestRunImportResults();
                                if (result.Count > 0)
                                {
                                    var resolver = _resolverFactory.GetResolver(settings, result, testCaseTestPlans);
                                    return resolver.ResolveTestCaseNames(errors);
                                }
                            }
                            catch (ApplicationException)
                            {
                                throw;
                            }
                            catch (XmlException ex)
                            {
                                throw new ApplicationException("Error parsing NUnit results XML file", ex);
                            }
                            catch (Exception ex)
                            {
                                throw new ApplicationException("Error importing NUnit results XML file", ex);
                            }
                        }
                    }
                }
            }
            catch (UriFormatException ex)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = "ResultsFilePath",
                    Message = ex.Message
                });
            }
            catch (ApplicationException ex)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = "ResultsFilePath",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = "ResultsFilePath",
                    Message = string.Format("Could not read file \"{0}\": {1}", settings.ResultsFilePath, ex.Message)
                });
            }
            return new CheckMappingResult { Errors = errors, NamesMappers = new List<NamesMapper>() };
        }
    }
}
