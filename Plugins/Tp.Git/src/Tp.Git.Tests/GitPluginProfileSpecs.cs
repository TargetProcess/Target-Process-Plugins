// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Testing.Common.NUnit;

namespace Tp.Git.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class GitPluginProfileSpecs
    {
        private GitPluginProfile _profile;
        private PluginProfileErrorCollection _errors;

        [SetUp]
        public void Init()
        {
            _profile = new GitPluginProfile() { Uri = "http://localhost", StartRevision = "1/1/1980" };
            _errors = new PluginProfileErrorCollection();
        }

        [Test]
        public void ShouldValidateStartRevisionShouldBeNotBeforeMin()
        {
            _profile.StartRevision = GitRevisionId.UtcTimeMin.AddDays(-1).ToString(CultureInfo.InvariantCulture.DateTimeFormat);
            _profile.Validate(_errors);

            var startRevisionError = _errors.Single();
            startRevisionError.FieldName.Should(Be.EqualTo("StartRevision"),
                "startRevisionError.FieldName.Should(Be.EqualTo(\"StartRevision\"))");
            startRevisionError.Message.Should(Be.EqualTo($"Start Revision Date should be not before {GitRevisionId.UtcTimeMin.ToShortDateString()}."),
                "startRevisionError.Message.Should(Be.EqualTo(\"Start Revision Date should be not before 1/1/1970\"))");
        }

        [Test]
        public void ShouldValidateStartRevisionShouldBeNotBehindMax()
        {
            _profile.StartRevision = GitRevisionId.UtcTimeMax.AddDays(1).ToString(CultureInfo.InvariantCulture.DateTimeFormat);
            _profile.Validate(_errors);

            var startRevisionError = _errors.Single();
            startRevisionError.FieldName.Should(Be.EqualTo("StartRevision"),
                "startRevisionError.FieldName.Should(Be.EqualTo(\"StartRevision\"))");
            startRevisionError.Message.Should(Be.EqualTo($"Start Revision Date should be not behind {GitRevisionId.UtcTimeMax.ToShortDateString()}."),
                "startRevisionError.Message.Should(Be.EqualTo(\"Start Revision Date should be not behind 1/19/2038\"))");
        }

        [Test]
        public void ShouldValidateInvalidStartRevision()
        {
            _profile.StartRevision = "that's not revision at all :(";
            _profile.Validate(_errors);

            var startRevisionError = _errors.Single();
            startRevisionError.FieldName.Should(Be.EqualTo("StartRevision"),
                "startRevisionError.FieldName.Should(Be.EqualTo(\"StartRevision\"))");
            startRevisionError.Message.Should(Be.EqualTo("Start Revision Date should be specified in mm/dd/yyyy format."),
                "startRevisionError.Message.Should(Be.EqualTo(\"Start Revision Date should be specified in mm/dd/yyyy format\"))");
        }

        [Test]
        public void ShouldValidateUriWithSpacesInTheBeginningAndInTheEnd()
        {
            ValidateUri("   //keeper/trunk  ");
        }

        [Test]
        public void ShouldValidateEmptyUri()
        {
            ValidateWrongUri(string.Empty, "Uri should not be empty.");
        }

        [Test]
        public void ShouldValidateUriConsistingOfWhitespaces()
        {
            ValidateWrongUri("   ", "Uri should not be empty.");
        }

        [Test]
        public void ShouldHandleLocalhostUri()
        {
            ValidateUri("http://localhost");
        }

        [Test]
        public void ShouldHandleKeeperUri()
        {
            ValidateUri("//keeper/trunk");
        }

        [Test]
        public void ShouldHandleGithubHttpsUri()
        {
            ValidateUri("https://git-hub.com/sll-uis/ngit.git/");
        }

        [Test]
        public void ShouldHandleGithubGitUri()
        {
            ValidateUri("git://github.com/sll-uis/ngit.git");
        }

        [Test]
        public void ShouldHandleGithubGitUriWithUsername()
        {
            ValidateUri("git://github.com/~username/sll-uis/ngit.git/");
        }

        [Test]
        public void ShouldHandleGithubGitUriWithDotInHostName()
        {
            ValidateUri("git@github.tpondemand.com:project/om3/core/logistics-portal/logistics-portal-api.git");
        }

        [Test]
        public void ShouldHandleFileUri()
        {
            ValidateUri("file:///path/to/so-me/repo.git/");
        }

        [Test]
        public void ShouldHandleUriWithDashes()
        {
            ValidateUri("//keeper-xps/Test-Repository/");
        }

        [Test]
        public void ShouldHandleInvalidUri1()
        {
            ValidateWrongUri("/bla-bla-bla", "Wrong Uri format.");
        }

        [Test]
        public void ShouldHandleInvalidUri2()
        {
            ValidateWrongUri("file:///", "Wrong Uri format.");
        }

        [Test]
        public void ShouldHandleInvalidUri3()
        {
            ValidateWrongUri("file://", "Wrong Uri format.");
        }

        [Test]
        public void ShouldHandleInvalidUri4()
        {
            ValidateWrongUri(@"d:\git_testing\TestRepository", "Wrong Uri format.");
        }

        [Test]
        public void ShouldHandleInvalidUriWithWhitespaces()
        {
            ValidateWrongUri("file:///path/to/r epo.git/", "Wrong Uri format.");
        }

        [Test]
        public void ShouldHandleSshUri()
        {
            ValidateUri("ssh://username@server.com:285/~username/path/to/repo.git");
        }

        [Test]
        public void ShouldHandleSshScpUri1()
        {
            ValidateUri("git@github.com:TargetProcess/RPG.git");
        }

        [Test]
        public void ShouldHandleSshScpUri2()
        {
            ValidateUri("github.com:TargetProcess/RPG.git");
        }

        private void ValidateUri(string uri)
        {
            _profile.Uri = uri;
            _profile.Validate(_errors);
            _errors.Should(Be.Empty, "_errors.Should(Be.Empty)");
        }

        private void ValidateWrongUri(string uri, string errorMessage)
        {
            _profile.Uri = uri;
            _profile.Validate(_errors);

            var startRevisionError = _errors.Single();

            startRevisionError.FieldName.Should(Be.EqualTo("Uri"), "startRevisionError.FieldName.Should(Be.EqualTo(\"Uri\"))");
            startRevisionError.Message.Should(Be.EqualTo(errorMessage), "startRevisionError.Message.Should(Be.EqualTo(errorMessage))");
        }
    }
}
