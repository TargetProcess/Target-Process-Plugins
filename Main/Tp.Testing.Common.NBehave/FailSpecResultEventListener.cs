// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBehave.Narrator.Framework;
using NUnit.Framework;

namespace Tp.Testing.Common.NBehave
{
    internal class FailSpecResultEventListener : IEventListener
    {
        private readonly List<ScenarioResult> _results = new List<ScenarioResult>();

        #region IEventListener Members

        public void FeatureCreated(string feature)
        {
        }

        public void FeatureNarrative(string message)
        {
        }

        public void ScenarioCreated(string scenarioTitle)
        {
        }

        public void RunStarted()
        {
        }

        public void RunFinished()
        {
            var failedScenario = _results.ToList().Find(x => x.Result is Failed);
            if (failedScenario != null)
            {
                var failed = failedScenario.ActionStepResults.ToList().FindAll(x => x.Result is Failed);
                var failMessage = new StringBuilder();
                var first = true;

                var failedResult = (Failed) failed[0].Result;

                foreach (var stepResult in failed)
                {
                    if (first)
                    {
                        // append error message only for the first failed step
                        failMessage.AppendLine($"Step '{stepResult.StringStep.Trim()}' failed with result:");
                        failMessage.AppendLine(failedResult.Exception.Message);
                        first = false;
                    }
                    else
                    {
                        failMessage.AppendLine(string.Empty);
                        failMessage.AppendLine($"Step '{stepResult.StringStep.Trim()}' failed");
                    }
                }

                failMessage.AppendLine(string.Empty);

                throw new Exception(failMessage.ToString(), failedResult.Exception);
            }

            _results.ToList().ForEach(
                x => Assert.That(x.Result, Is.InstanceOf<Passed>(), x.Message + ": " + Environment.NewLine + x.StackTrace));
        }

        public void ThemeStarted(string name)
        {
        }

        public void ThemeFinished()
        {
        }

        public void ScenarioResult(ScenarioResult result)
        {
            _results.Add(result);
        }

        #endregion
    }
}
