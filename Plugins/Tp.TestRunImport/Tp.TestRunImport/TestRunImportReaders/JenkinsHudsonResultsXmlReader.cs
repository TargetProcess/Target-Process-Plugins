// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
    public class JenkinsHudsonResultsXmlReader : AbstractTestRunImportResultsReader
    {
        public JenkinsHudsonResultsXmlReader(IActivityLogger log, StreamReader reader)
            : base(log, reader)
        {
        }

        public override List<TestRunImportResultInfo> GetTestRunImportResults()
        {
            using (var reader = XmlReader.Create(Reader))
            {
                var result = new List<TestRunImportResultInfo>();

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);

                var testCases = xmlDocument.SelectNodes("//suite/case");
                if (testCases == null) return result;
                var runDate = DateTime.Now;

                foreach (XmlNode testCase in testCases)
                {
                    if (testCase.ChildNodes.Count == 0) continue;

                    var testNameNode = testCase.SelectSingleNode("name");
                    if (testNameNode == null || string.IsNullOrEmpty(testNameNode.InnerText)) continue;

                    var isSkippedNode = testCase.SelectSingleNode("skipped");
                    var isSuccessNode = testCase.SelectSingleNode("status");

                    bool isSkippedForJenkinsHudson = true;
                    if (isSkippedNode != null && !string.IsNullOrEmpty(isSkippedNode.InnerText))
                    {
                        bool.TryParse(isSkippedNode.InnerText.Trim(), out isSkippedForJenkinsHudson);
                    }

                    bool? isSuccess = null;
                    if (!isSkippedForJenkinsHudson && isSuccessNode != null && !string.IsNullOrEmpty(isSuccessNode.InnerText))
                    {
                        bool isSuccessTemp = string.Compare(isSuccessNode.InnerText.Trim(), "passed",
                            StringComparison.InvariantCultureIgnoreCase) == 0;
                        isSuccess = isSuccessTemp;
                    }
                    result.Add(new TestRunImportResultInfo { Name = testNameNode.InnerText, IsSuccess = isSuccess, RunDate = runDate });
                }
                return result;
            }
        }
    }
}
