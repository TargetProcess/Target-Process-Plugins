// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Net;
using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.TestRunImport.Mappers;
using Tp.Integration.Plugin.TestRunImport.Streams;
using Tp.Integration.Plugin.TestRunImport.TestCaseResolvers;
using Tp.Integration.Plugin.TestRunImport.TestRunImportReaders;

namespace Tp.Integration.Plugin.TestRunImport.StructureMap
{
    public class PluginRegistry : Registry
    {
        public PluginRegistry()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            For<ITestRunImportResultsReaderFactory>().Singleton().Use<SimpleTestRunImportResultsReaderFactory>();
            For<ITestCaseResolverFactory>().Singleton().Use<SimpleTestCaseResolverFactory>();
            For<IStreamFactory>().Singleton().Use<SimpleStreamFactory>();
            For<IMappingProfileValidator>().Use<MappingProfileProfileValidator>();
            For<IMappingChecker>().Use<MappingChecker>();
        }
    }
}
