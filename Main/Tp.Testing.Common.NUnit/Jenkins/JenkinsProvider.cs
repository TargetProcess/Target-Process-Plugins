using System;

namespace Tp.Testing.Common.NUnit.Jenkins
{
    public static class JenkinsProvider
    {
        public static string JenkinsUrl => Environment.GetEnvironmentVariable("JENKINS_URL");
        public static bool IsRunningOnJenkins => !string.IsNullOrWhiteSpace(JenkinsUrl);
    }
}
