// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using NBehave.Narrator.Framework;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Plugin.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;

namespace Tp.TestRunImport.Tests.Selenium
{
    [ActionSteps]
    public class SeleniumTestRunImportActionSteps : ImportResultsTestRunImportActionSteps
    {
        [Given("post results to remote Url")]
        public void PostResultsToRemoteUrl()
        {
            Settings.PostResultsToRemoteUrl = true;
        }

        [Given("command SeleniumResults is sent to TargetProcess")]
        public void SeleniumResultsCommandIsSent()
        {
            var command = new ExecutePluginCommandCommand
            {
                CommandName = "SeleniumResults",
                Arguments = ReadSeleniumResourceFileForCurrentProfile()
            };
            Context.Transport.HandleMessageFromTp(Context.CurrentProfile, command);
        }

        [Given("results remote Url is '$remoteUrl'")]
        public void ResultsRemoteUrl(string remoteUrl)
        {
            Settings.RemoteResultsUrl = remoteUrl;
        }

        [Given("authentication user Id is '$authTokenUserId'")]
        public void AuthTokenUserId(int authTokenUserId)
        {
            Settings.AuthTokenUserId = authTokenUserId;
        }

        private string ReadSeleniumResourceFileForCurrentProfile()
        {
            var builder = new StringBuilder();
            using (var stream = GetTypeManifestResourceStream())
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        string input;
                        while ((input = sr.ReadLine()) != null)
                        {
                            builder.Append(input);
                        }
                        sr.Close();
                    }
                    stream.Close();
                }
            }
            return HttpUtility.UrlEncode(builder.ToString());
        }

        private Stream GetTypeManifestResourceStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"{TypeResourceLocation}.{ResourceFileNameForCurrentProfile}");
        }

        private string ResourceFileNameForCurrentProfile
        {
            get
            {
                var profile = Context.CurrentProfile.GetProfile<TestRunImportPluginProfile>();
                if (string.IsNullOrEmpty(profile.RegExp))
                {
                    return SimpleResultsFile;
                }
                if (profile.RegExp.Contains("<testId>"))
                {
                    return TestIdRegExpResultsFile;
                }
                return profile.RegExp.Contains("<testName>") ? TestNameRegExpResultsFile : string.Empty;
            }
        }

        protected override FrameworkTypes FrameworkType => FrameworkTypes.Selenium;

        private string SimpleResultsFile => "SimpleSeleniumTestResult.txt";

        private string TestIdRegExpResultsFile => "TestIdRegExpSeleniumTestResult.txt";

        private string TestNameRegExpResultsFile => "TestNameRegExpSeleniumTestResult.txt";
    }
}
