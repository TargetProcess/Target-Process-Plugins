// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.Utils;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
	public class SeleniumResultsHtmlReader : AbstractTestRunImportResultsReader
	{
		public SeleniumResultsHtmlReader(IActivityLogger log, TextReader reader)
			: base(log, reader)
		{
		}

		public override List<TestRunImportResultInfo> GetTestRunImportResults()
		{
			var result = new List<TestRunImportResultInfo>();
			if (Reader != null)
			{
				var runDate = DateTime.Now;
				using (var htmlReader = new HtmlReader(Reader))
				{
					// move over all theads
					while (htmlReader.ReadToFollowing("thead"))
					{
						if (htmlReader.ReadToFollowing("tr") && htmlReader.Attributes.Count > 0)
						{
							var classAttribute = htmlReader.Attributes["class"];
							if (!string.IsNullOrEmpty(classAttribute) && classAttribute.Contains("status_"))
							{
								var table = ParseTrTag(htmlReader);
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
			return result;
		}

		private static TestRunImportResultInfo ParseTrTag(HtmlReader htmlReader)
		{
			var parsed = false;
			var testCaseName = string.Empty;
			var testCaseSucceeded = false;

			var attribute = htmlReader.Attributes["class"];
			if (attribute != null)
			{
				var strings = attribute.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var status in from s in strings where s.StartsWith("status_") select s.Substring("status_".Length))
				{
					testCaseSucceeded = string.Compare(status, "passed", StringComparison.InvariantCulture) == 0;
					break;
				}
				if (htmlReader.ReadToFollowing("td"))
				{
					testCaseName = htmlReader.GetInnerTextUpToElement("td", HtmlNodeType.EndElement);
					parsed = true;
				}
			}
			return parsed ? new TestRunImportResultInfo {Name = testCaseName, IsSuccess = testCaseSucceeded} : null;
		}
	}
}