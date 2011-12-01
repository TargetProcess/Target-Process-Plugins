// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
	public class JUnitResultsXmlReader : AbstractTestRunImportResultsReader
	{
		protected internal JUnitResultsXmlReader(IActivityLogger log, TextReader reader)
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

				var testCases = xmlDocument.SelectNodes("//testcase");
				if (testCases == null) return result;
				var runDate = DateTime.Now;

				foreach (XmlNode testCase in testCases)
				{
					if (testCase.Attributes == null) continue;

					var testNameAttr = testCase.Attributes["name"];
					if (testNameAttr == null) continue;

					var isIncomplete = false;
					var isIncompleteAttr = testCase.Attributes["incomplete"];
					var isIgnoredAttr = testCase.Attributes["ignored"];
					if ((isIncompleteAttr != null && isIncompleteAttr.Value == "true") ||
						(isIgnoredAttr != null && isIgnoredAttr.Value == "true"))
						isIncomplete = true;

					bool? isSuccess = null;
					if (!isIncomplete)
						isSuccess = true;

					isSuccess &= !TestCaseNodeContainsChild(testCase, "failure");
					isSuccess &= !TestCaseNodeContainsChild(testCase, "error");

					result.Add(new TestRunImportResultInfo { Name = testNameAttr.Value, IsSuccess = isSuccess, RunDate = runDate });
				}
				return result;
			}
		}

		private static bool TestCaseNodeContainsChild(XmlNode node, string tagName)
		{
			return node.ChildNodes.Cast<XmlNode>().Any(childNode => childNode.Name == tagName);
		}
	}
}