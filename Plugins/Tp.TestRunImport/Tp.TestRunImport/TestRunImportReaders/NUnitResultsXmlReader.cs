// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
    public class NUnitResultsXmlReader : AbstractTestRunImportResultsReader
    {
        protected internal NUnitResultsXmlReader(IActivityLogger log, TextReader reader)
            : base(log, reader)
        {
        }

        public override List<TestRunImportResultInfo> GetTestRunImportResults()
        {
            using (var reader = XmlReader.Create(Reader))
            {
                var result = new List<TestRunImportResultInfo>();
                while (reader.Read())
                {
                    if (reader.Name == "test-results")
                    {
                        result.AddRange(ParseNUnit20Results(reader));
                    }
                    if (reader.Name == "test-run")
                    {
                        result.AddRange(ParseNUnit30Results(reader));
                    }
                }
                return result;
            }
        }

        private static IEnumerable<TestRunImportResultInfo> ParseNUnit20Results(XmlReader reader)
        {
            var result = new List<TestRunImportResultInfo>();

            var dateAttr = reader.GetAttribute("date");
            var timeAttr = reader.GetAttribute("time");
            var runDate = DateTime.Now;

            DateTime datePart;
            if (DateTime.TryParseExact(dateAttr, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None,
                out datePart))
            {
                DateTime timePart;
                if (DateTime.TryParseExact(timeAttr, "HH:mm:ss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None,
                    out timePart))
                {
                    runDate = datePart.Date + timePart.TimeOfDay;
                }
            }

            while (reader.Read())
            {
                if (reader.Name != "test-case") continue;
                if (!reader.HasAttributes) continue;

                var testNameAttr = reader.GetAttribute("name");
                var isExecutedAttr = reader.GetAttribute("executed");
                var isSuccessAttr = reader.GetAttribute("success");

                bool isExecutedForNUnit;
                bool.TryParse(isExecutedAttr, out isExecutedForNUnit);

                bool? isSuccess = null;
                if (isExecutedForNUnit)
                {
                    bool isSuccessTemp;
                    bool.TryParse(isSuccessAttr, out isSuccessTemp);
                    isSuccess = isSuccessTemp;
                }
                result.Add(new TestRunImportResultInfo { Name = testNameAttr, IsSuccess = isSuccess, RunDate = runDate });
            }
            return result;
        }

        private static IEnumerable<TestRunImportResultInfo> ParseNUnit30Results(XmlReader reader)
        {
            var result = new List<TestRunImportResultInfo>();

            while (reader.Read())
            {
                if (reader.Name != "test-case") continue;
                if (!reader.HasAttributes) continue;

                var runDate = DateTime.Now;
                var startTimeAttr = reader.GetAttribute("start-time");
                if (!string.IsNullOrEmpty(startTimeAttr))
                {
                    if (!DateTime.TryParseExact(startTimeAttr, "yyyy-MM-dd HH:mm:ssZ", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None,
                        out runDate))
                    {
                        runDate = DateTime.Now;
                    }
                }
                var testNameAttr = reader.GetAttribute("fullname");
                var runStateAttr = reader.GetAttribute("runstate");//Ignored,Runnable
                var resultAttr = reader.GetAttribute("result");//Failed,Skipped,Passed

                bool isExecutedForNUnit = string.Compare(runStateAttr, "Runnable", StringComparison.OrdinalIgnoreCase) == 0;

                bool? isSuccess = null;
                if (isExecutedForNUnit)
                {
                    bool isSuccessTemp = string.Compare(resultAttr, "Passed", StringComparison.OrdinalIgnoreCase) == 0;
                    isSuccess = isSuccessTemp;
                }
                result.Add(new TestRunImportResultInfo { Name = testNameAttr, IsSuccess = isSuccess, RunDate = runDate });
            }
            return result;
        }
    }
}
