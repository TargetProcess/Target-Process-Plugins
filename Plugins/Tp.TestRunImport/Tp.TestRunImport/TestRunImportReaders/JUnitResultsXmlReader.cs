// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
			
					var error = new StringBuilder();

					var failureNode = GetChildNode(testCase, "failure");
					if (failureNode != null)
					{
						isSuccess = false;
						AppendCommentMessage(failureNode, error);
					}

					var errorNode = GetChildNode(testCase, "error");
					if (errorNode != null)
					{
						isSuccess = false;
						AppendCommentMessage(errorNode, error);
					}

					result.Add(new TestRunImportResultInfo { Name = testNameAttr.Value, IsSuccess = isSuccess, RunDate = runDate, Comment = (isSuccess.HasValue && !isSuccess.Value) ? error.ToString() : null });
				}
				return result;
			}
		}

		private static XmlNode GetChildNode(XmlNode node, string tagName)
		{
			return node.HasChildNodes ? node.ChildNodes.Cast<XmlNode>().FirstOrDefault(childNode => childNode.Name == tagName) : null;
		}

		private static void AppendCommentMessage(XmlNode failureNode, StringBuilder errorBuilder)
		{
			if (errorBuilder.Length > 0)
			{
				errorBuilder.AppendLine("<div><br /></div>");
			}

			var type = failureNode.Attributes != null ? failureNode.Attributes["type"] : null;
			if (type != null && !string.IsNullOrEmpty(type.Value))
			{
				var typeMessage = type.Value.Trim('\r').TrimStart(new[] { '\n', ' ' }).TrimEnd(new[] { '\n', ' ' });
				if (!string.IsNullOrEmpty(typeMessage))
				{
					foreach (var line in typeMessage.Split('\n'))
					{
						errorBuilder.AppendFormat("<div>{0}</div>", line);
					}
				}
			}

			var message = failureNode.Attributes != null ? failureNode.Attributes["message"] : null;
			if (message != null && !string.IsNullOrEmpty(message.Value))
			{
				var errorMessage = message.Value.Trim('\r').TrimStart(new[] { '\n', ' ' }).TrimEnd(new[] { '\n', ' ' });
				if (!string.IsNullOrEmpty(errorMessage))
				{
					errorBuilder.AppendLine("<div><br /></div>");
					foreach (var line in errorMessage.Split('\n'))
					{
						errorBuilder.AppendFormat("<div>{0}</div>", line);
					}
				}
			}
		}
	}
}