// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBehave.Narrator.Framework;

namespace Tp.Testing.Common.NBehave
{
	public class ColorfulConsoleOutputEventListener : IEventListener
	{
		private List<ScenarioResult> _allResults = new List<ScenarioResult>();

		void IEventListener.RunStarted()
		{
			_allResults = new List<ScenarioResult>();
		}

		public void FeatureCreated(string feature)
		{
			if (string.IsNullOrEmpty(feature))
			{
				Console.WriteLine();
				return;
			}
			WriteColorString("Feature: " + feature, ConsoleColor.Cyan);
		}

		void IEventListener.FeatureNarrative(string narrative)
		{
			if (string.IsNullOrEmpty(narrative))
			{
				return;
			}
			WriteColorString(narrative, ConsoleColor.DarkCyan);
		}

		void IEventListener.ScenarioCreated(string scenarioTitle) {}

		void IEventListener.RunFinished()
		{
			Console.WriteLine("");
			Console.ResetColor();
			var failureText = new StringBuilder("-----------------------------------------" + Environment.NewLine);
			foreach (var failedActionStepResult in _allResults.Where(x => x.Result is Failed))
			{
				failureText.AppendLine(string.Format("Feature: {0}", failedActionStepResult.FeatureTitle));
				failureText.AppendLine(string.Format("  Scenario: {0}", failedActionStepResult.ScenarioTitle));
				foreach (var actionStepResult in failedActionStepResult.ActionStepResults)
				{
					WriteFailure(failureText, failedActionStepResult, actionStepResult);
				}
			}
			Console.WriteLine(failureText);
		}

		private static void WriteFailure(StringBuilder failureText, ScenarioResult failedActionStepResult, ActionStepResult actionStepResult)
		{
			if (!(actionStepResult.Result is Failed))
			{
				return;
			}

			failureText.AppendLine(actionStepResult.StringStep.Trim('\t') + " - " + actionStepResult.Result);
			failureText.AppendLine(((Failed) failedActionStepResult.Result).Exception.StackTrace);
		}


		void IEventListener.ThemeStarted(string name) {}

		void IEventListener.ThemeFinished() {}

		void IEventListener.ScenarioResult(ScenarioResult scenarioResult)
		{
			WriteColorString("Scenario: " + scenarioResult.ScenarioTitle + " - " + scenarioResult.Result.ToString().ToUpper(), GetColorForResult(scenarioResult.Result));
			_allResults.Add(scenarioResult);
			foreach (var stepResult in scenarioResult.ActionStepResults)
			{
				string stepResultSuffix = stepResult.Result is Passed ? string.Empty : " - " + stepResult.Result.ToString().ToUpper();

				string result = stepResult.StringStep + stepResultSuffix;

				WriteColorString(result, GetColorForResult(stepResult.Result));
			}

			DoExamplesInScenario(scenarioResult as ScenarioExampleResult);
		}


		private static void DoExamplesInScenario(ScenarioExampleResult scenarioExampleResult)
		{
			if (scenarioExampleResult == null)
			{
				return;
			}

			string columns = "Examples:" + Environment.NewLine + "|";
			foreach (var columnName in scenarioExampleResult.Examples.First().ColumnNames)
			{
				columns += columnName + "|";
			}
			WriteColorString(columns, ConsoleColor.Gray);

			var scenarioResults = scenarioExampleResult.ExampleResults.ToArray();
			int idx = 0;
			foreach (var example in scenarioExampleResult.Examples)
			{
				string row = "|";
				foreach (var columnName in example.ColumnNames)
				{
					row += example.ColumnValues[columnName] + "|";
				}
				WriteColorString(row, GetColorForResult(scenarioResults[idx++].Result));
			}
		}


		private static void WriteColorString(string text, ConsoleColor color)
		{
			text = text.TrimStart('\t');
			var tabs = "  ";

			if (text.StartsWith("Given") || text.StartsWith("When") || text.StartsWith("Then"))
			{
				tabs += "  ";
			}
			else if (text.StartsWith("And"))
			{
				tabs += "    ";
			}

			text = string.Format("{0}{1}", tabs, text);

			var currentColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(text);
			Console.ForegroundColor = currentColor;
		}

		private static ConsoleColor GetColorForResult(Result result)
		{
			if (result is Passed)
			{
				return ConsoleColor.Green;
			}
			if (result is Failed)
			{
				return ConsoleColor.Red;
			}
			if (result is Pending)
			{
				return ConsoleColor.Yellow;
			}

			return ConsoleColor.Gray;
		}
	}
}