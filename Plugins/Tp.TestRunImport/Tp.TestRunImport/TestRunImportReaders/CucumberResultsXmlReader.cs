using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
    public class CucumberResultsXmlReader : AbstractTestRunImportResultsReader
    {
        public CucumberResultsXmlReader(IActivityLogger log, StreamReader reader) : base(log, reader)
        {
        }

        public override List<TestRunImportResultInfo> GetTestRunImportResults()
        {
            using (var reader = XmlReader.Create(Reader))
            {
                var result = new List<TestRunImportResultInfo>();

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);

                var testCases = xmlDocument.SelectNodes("//testcase");
                if (testCases == null) return result;
                var runDate = DateTime.Now;

                foreach (XmlNode testCase in testCases)
                {
                    var testNameAttr = testCase.Attributes?["name"];
                    if (testNameAttr == null) continue;

                    bool? isSuccess = null;
                    string errorMessage = null;

                    var failure = testCase.SelectNodes("failure");
                    if (failure != null && failure.Count > 0)
                    {
                        isSuccess = false;
                        errorMessage = failure[0].Attributes?["message"].Value;
                    }
                    else
                    {
                        var systemOuts = testCase.SelectNodes("system-out");

                        if (systemOuts != null && systemOuts.Count > 0)
                        {
                            isSuccess = true;
                            foreach (XmlNode systemOut in systemOuts)
                            {
                                using (var outReader = new StringReader(systemOut.InnerText))
                                {
                                    string line;
                                    while (isSuccess.Value && (line = outReader.ReadLine()) != null)
                                    {
                                        isSuccess = line.EndsWith(".passed", StringComparison.OrdinalIgnoreCase);
                                    }
                                }
                            }
                        }
                    }

                    result.Add(new TestRunImportResultInfo
                    {
                        Name = testNameAttr.Value,
                        IsSuccess = isSuccess,
                        Comment = errorMessage,
                        RunDate = runDate
                    });
                }

                return result;
            }
        }
    }
}
