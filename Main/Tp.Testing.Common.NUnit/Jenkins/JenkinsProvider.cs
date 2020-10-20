using System;

namespace Tp.Testing.Common.NUnit.Jenkins
{
    public static class JenkinsProvider
    {
        public static string JenkinsUrl => Environment.GetEnvironmentVariable("JENKINS_URL");

        public static bool IsRunningOnJenkins =>
            !string.IsNullOrWhiteSpace(JenkinsUrl)
            // Temporary assumption as long as we support runners deployed both to AWS and in office environment
            // Dumb and naive observation: office runner machines are named like CI-TESTS-***
            || (Environment.GetEnvironmentVariable("COMPUTERNAME") is string computerName && computerName.StartsWith("CI-TESTS-"));
    }
}
