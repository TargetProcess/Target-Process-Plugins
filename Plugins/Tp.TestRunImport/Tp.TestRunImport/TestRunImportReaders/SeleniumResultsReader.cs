// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
    public class SeleniumResultsReader : AbstractTestRunImportResultsReader
    {
        public SeleniumResultsReader(IActivityLogger log, StreamReader reader)
            : base(log, reader)
        {
        }

        // Display any warnings or errors.
        public override List<TestRunImportResultInfo> GetTestRunImportResults()
        {
            var result = new List<TestRunImportResultInfo>();
            if (Reader != null)
            {
                var runDate = DateTime.Now;

                using (var reader = XmlReader.Create(Reader))
                {
                    try
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.Load(reader);

                        var testCases = xmlDocument.SelectNodes("//test-method");
                        if (testCases == null) return result;

                        foreach (XmlNode testCase in testCases)
                        {
                            var testNameAttr = testCase.Attributes?["name"];
                            if (testNameAttr == null) continue;

                            var status = testCase.Attributes?["status"];

                            bool? isSuccess = null;
                            string comment = null;

                            if (status != null)
                            {
                                switch (status.Value)
                                {
                                    case "PASS":
                                        isSuccess = true;
                                        break;
                                    case "FAIL":
                                    {
                                        isSuccess = false;
                                        var error = testCase.SelectSingleNode("//short-stacktrace");
                                        if (error != null && error.ChildNodes.Count > 0)
                                        {
                                            var commentBuilder = new StringBuilder();
                                            foreach (XmlNode e in error.ChildNodes)
                                            {
                                                commentBuilder.AppendLine(e.Value);
                                            }
                                            comment = commentBuilder.ToString();
                                        }
                                        break;
                                    }
                                }
                            }

                            result.Add(new TestRunImportResultInfo
                            {
                                Name = testNameAttr.Value,
                                IsSuccess = isSuccess,
                                RunDate = runDate,
                                Comment = comment
                            });
                        }
                    }
                    catch (XmlException)
                    {
                        result.Clear();

                        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.Load(Reader);
                        foreach (var node in htmlDocument.DocumentNode.SelectNodes("//thead"))
                        {
                            var tr = node.SelectSingleNode("tr");
                            if (tr.Attributes.Count > 0)
                            {
                                var classAttribute = tr.Attributes["class"];
                                if (classAttribute?.Value != null && classAttribute.Value.Contains("status_"))
                                {
                                    var table = ParseTrTag(tr);
                                    if (table != null)
                                    {
                                        table.RunDate = runDate;
                                        result.Add(table);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static TestRunImportResultInfo ParseTrTag(HtmlNode tr)
        {
            var parsed = false;
            var testCaseName = string.Empty;
            var testCaseSucceeded = false;

            var attribute = tr.Attributes["class"];
            if (attribute != null)
            {
                var strings = attribute.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var status in from s in strings where s.StartsWith("status_") select s.Substring("status_".Length))
                {
                    testCaseSucceeded = string.Compare(status, "passed", StringComparison.InvariantCulture) == 0;
                    break;
                }

                var td = tr.SelectSingleNode("td");
                if (td != null)
                {
                    testCaseName = td.InnerText;
                    parsed = true;
                }
            }

            return parsed ? new TestRunImportResultInfo { Name = testCaseName, IsSuccess = testCaseSucceeded } : null;
        }
    }
}
